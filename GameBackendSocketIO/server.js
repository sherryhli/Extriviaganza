if (process.env.NODE_ENV !== 'production') {
    require('dotenv').config();
}

const app = require('express')();
const http = require('http').createServer(app);
const io = require('socket.io')(http);
const mongoClient = require('mongodb').MongoClient;
const redis = require('redis');
const REDIS_URL = process.env.REDIS_URL || 'redis://127.0.0.1:6379';
const redisClient = redis.createClient(REDIS_URL);
const axios = require('axios');

const PORT = process.env.PORT || 3000;
const DEFAULT_NAMESPACE = '/';
const QBQUESTIONS_API_BASE_URL = 'http://qbquestionsapi.azurewebsites.net/api';
const REDIS_QUESTIONS_KEY = 'questions';

// Question level constants
const MIDDLE_SCHOOL = 'MiddleSchool';
const HIGH_SCHOOL = 'HighSchool';
const COLLEGIATE = 'Collegiate';
const TRASH = 'Trash';


// For testing purposes only, will be creating a client in separate repo
app.get('/test1', (req, res) => {
    res.sendFile(__dirname + '/test1.html');
});

app.get('/test2', (req, res) => {
    res.sendFile(__dirname + '/test2.html');
});


var database, collection;

http.listen(PORT, () => {
    console.log(`Listening on *:${PORT}`);
    mongoClient.connect(process.env.MONGO_CONNECTION_STRING, { useNewUrlParser: true, useUnifiedTopology: true }, (error, client) => {
        if (error) {
            console.log(`Could not connect to database, error = ${error.message} and ${error.errmsg}`);
            throw error;
        }
        database = client.db(process.env.MONGO_DB_NAME);
        collection = database.collection(process.env.MONGO_COLLECTION_NAME);
        console.log('Connected to database');
    });

    redisClient.on('connect', () => {
        console.log('Connected to Redis server');
    });
});


function getSocketsInGame(gameId) {
    return Object.values(io.of(DEFAULT_NAMESPACE).connected).filter(s => s.gameId === gameId);
}


io.origins(['http://extriviaganza.herokuapp.com']);

io.on('connection', socket => {
    console.log(`A user connected, socketId = ${socket.id}`);

    socket.on('join game', (gameId, userId, existingGame) => {
        socket.join(gameId);
        // adding gameId as a custom property to socket
        socket.gameId = gameId;

        collection.findOne({ "gameId": gameId }, (error, result) => {
            if (error) {
                socket.emit('fatal error');
                socket.leave(gameId, () => {
                    socket.disconnect(true);
                });
            } else {
                if (result) {
                    if (result.players.filter(p => p.userId === userId).length !== 0) {
                        console.log(`UserId = ${userId} is already in use`);
                        socket.emit('duplicate userId', userId);
                        return;
                    }

                    console.log('Adding player to existing game');
                    const newPlayer = {
                        "userId": userId,
                        "score": 0,
                        "socketId": socket.id
                    }

                    collection.findOneAndUpdate(
                        { "gameId": gameId },
                        { $push: { "players": newPlayer } },
                        { returnOriginal: false },
                        (error, result) => {
                            if (error) {
                                socket.emit('fatal error');
                                socket.leave(gameId, () => {
                                    socket.disconnect(true);
                                });
                            } else {
                                io.sockets.in(gameId).emit('player joined', result.value);
                            }
                        }
                    );
                } else if (!existingGame) {
                    console.log('Creating new game and adding player to it');

                    // fetch auth token and save to DB
                    axios.post(`${QBQUESTIONS_API_BASE_URL}/authenticate`, {
                        "username": process.env.QBQUESTIONS_API_WEB_USERNAME,
                        "password": process.env.QBQUESTIONS_API_WEB_PASSWORD
                    }).then(response => {
                        const game = {
                            "gameId": gameId,
                            "token": response.data,
                            "creationTime": Math.round((new Date()).getTime() / 1000),
                            "players": [
                                {
                                    "userId": userId,
                                    "score": 0,
                                    "socketId": socket.id
                                }
                            ]
                        };
                        collection.insertOne(game, (error, result) => {
                            if (error) {
                                socket.emit('fatal error');
                                socket.leave(gameId, () => {
                                    socket.disconnect(true);
                                });
                            } else {
                                io.sockets.in(gameId).emit('player joined', result.ops[0]);
                            }
                        });
                    }).catch(error => {
                        // unable to fetch auth token
                        socket.emit('fatal error');
                        socket.leave(gameId, () => {
                            socket.disconnect(true);
                        });
                    });
                } else {
                    console.log(`Attempted to join existing game but game with gameId = ${gameId} does not exist`);
                    socket.emit('fatal error');
                    socket.leave(gameId, () => {
                        socket.disconnect(true);
                    });
                }
            }
        });
    });


    socket.on('get question', (level) => {
        io.sockets.in(socket.gameId).emit('re-enable buzz');

        const levelQueryParam = level ? `?level=${level}` : '';

        // randomly select a level for Redis query if not supplied
        if (!level) {
            const random = Math.floor((Math.random() * 4));
            switch (random) {
                case 0:
                    level = MIDDLE_SCHOOL;
                    break;
                case 1:
                    level = HIGH_SCHOOL;
                    break;
                case 2:
                    level = COLLEGIATE;
                    break;
                case 3:
                    level = TRASH;
                    break;
            }
        }

        const REDIS_QUESTIONS_SET_KEY = `${REDIS_QUESTIONS_KEY}:${level}`;

        redisClient.spop(REDIS_QUESTIONS_SET_KEY, (err, reply) => {
            let question = null;
            let parseError = false;

            if (reply) {
                try {
                    question = JSON.parse(reply);
                } catch (error) {
                    parseError = true;
                }
            }

            if (err || !reply || parseError) {
                console.log('Fetching question from API');
                // TODO: store token in Redis so MongoDB query can be avoided? Look into security of Heroku Redis
                // This means worker process would need to ensure there is always valid token available
                collection.findOne({ "gameId": socket.gameId }, (error, result) => {
                    if (error) {
                        io.sockets.in(socket.gameId).emit('retryable error');
                    } else {
                        if (result) {
                            const url = `${QBQUESTIONS_API_BASE_URL}/qbquestions/random${levelQueryParam}`;
                            const auth = `Bearer ${result.token}`;

                            axios.get(url, { headers: { Authorization: auth } })
                                .then(response => {
                                    io.sockets.in(socket.gameId).emit('receive question', response.data);
                                }).catch(error => {
                                    console.log('Error retrieving question from API');
                                    console.log(error);
                                    io.sockets.in(socket.gameId).emit('retryable error');
                                });
                        } else {
                            console.log(`${socket.gameId} is not found in the DB`);
                            io.sockets.in(socket.gameId).emit('fatal error');
                            const socketsInGame = getSocketsInGame(socket.gameId);
                            socketsInGame.forEach(s => {
                                s.leave(socket.gameId, () => {
                                    s.disconnect(true);
                                })
                            });
                        }
                    }
                });
            } else {
                console.log('Fetched question from Redis');
                io.sockets.in(socket.gameId).emit('receive question', question);
            }
        });
    });


    socket.on('buzz', (userId) => {
        socket.emit('buzz acknowledged');
        socket.broadcast.to(socket.gameId).emit('other player buzzed', `${userId} buzzed`);
    });


    socket.on('correct answer', (userId, power) => {
        const points = power ? 15 : 10;

        collection.findOneAndUpdate(
            { "gameId": socket.gameId, "players.userId": userId },
            { $inc: { "players.$.score": points } },
            { returnOriginal: false },
            (error, result) => {
                if (error) {
                    io.sockets.in(socket.gameId).emit('retryable error');
                } else {
                    io.sockets.in(socket.gameId).emit('player answered correctly', result.value);
                }
            }
        );
    });


    socket.on('incorrect answer', (userId, answer, power) => {
        if (!power) {
            // sending null as last argument since game state does not need to be updated
            io.sockets.in(socket.gameId).emit('player answered incorrectly', userId, answer, null);
        } else {
            // incorrect answer on power is an "interrupt" and there's a 5 point penalty
            collection.findOneAndUpdate(
                { "gameId": socket.gameId, "players.userId": userId },
                { $inc: { "players.$.score": -5 } },
                { returnOriginal: false },
                (error, result) => {
                    if (error) {
                        io.sockets.in(socket.gameId).emit('retryable error');
                    } else {
                        io.sockets.in(socket.gameId).emit('player answered incorrectly', userId, answer, result.value);
                    }
                }
            );
        }
    });


    socket.on('disconnect', () => {
        console.log('user disconnected');
        collection.findOne({ "gameId": socket.gameId }, (error, result) => {
            if (error) {
                console.log('Player not removed from game in MongoDB');
            } else {
                if (result) {
                    const remainingPlayers = result.players.filter(p => p.socketId !== socket.id);
                    if (remainingPlayers.length === 0) {
                        // remove the game from MongoDB if all players have left
                        collection.deleteOne({ "gameId": socket.gameId });
                    } else {
                        collection.findOneAndUpdate(
                            { "gameId": socket.gameId },
                            { $pull: { "players": { "socketId": socket.id } } },
                            { returnOriginal: false },
                            (error, result) => {
                                if (error) {
                                    console.log('Player not removed from game in MongoDB');
                                } else {
                                    io.sockets.in(socket.gameId).emit('player joined', result.value);
                                }
                            }
                        );
                    }
                } else {
                    console.log('Player not removed from game in MongoDB');
                }
            }
        });
    });
});
