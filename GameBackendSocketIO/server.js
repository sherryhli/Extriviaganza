if (process.env.NODE_ENV !== 'production') {
    require('dotenv').config();
}

const app = require('express')();
const http = require('http').createServer(app);
const io = require('socket.io')(http);
const mongoClient = require("mongodb").MongoClient;
const axios = require('axios');

const PORT = process.env.PORT || 3000;
const DEFAULT_NAMESPACE = '/';


// For testing purposes only, will be creating a client in separate repo
app.get('/test1', function (req, res) {
    res.sendFile(__dirname + '/test1.html');
});

app.get('/test2', function (req, res) {
    res.sendFile(__dirname + '/test2.html');
});


var database, collection;

http.listen(PORT, function () {
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
});


function getSocketsInGame(gameId) {
    return Object.values(io.of(DEFAULT_NAMESPACE).connected).filter(s => s.gameId === gameId);
}


io.on('connection', function (socket) {
    console.log(`A user connected, socketId = ${socket.id}`);

    socket.on('join game', function (gameId, userId) {
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
                    result.players.push(newPlayer);

                    collection.findOneAndUpdate(
                        { "gameId": gameId },
                        { $set: { "players": result.players } },
                        { returnOriginal: false },
                        function (error, result) {
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
                } else {
                    console.log('Creating new game and adding player to it');

                    // fetch auth token and save to DB
                    axios.post('http://qbquestionsapi.azurewebsites.net/api/authenticate', {
                        "username": process.env.QBQUESTIONS_API_USERNAME,
                        "password": process.env.QBQUESTIONS_API_PASSWORD
                    }).then(response => {
                        console.log(response.data);

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
                        collection.insertOne(game, function (error, result) {
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
                }
            }
        });
    });


    socket.on('get question', function (gameId, level) {
        const levelQueryParam = level ? `?level=${level}` : '';
        const url = `http://qbquestionsapi.azurewebsites.net/api/qbquestions/random${levelQueryParam}`;

        collection.findOne({ "gameId": gameId }, (error, result) => {
            if (error) {
                io.sockets.in(gameId).emit('retryable error');
            } else {
                if (result) {
                    const auth = `Bearer ${result.token}`;
                    axios.get(url, { headers: { Authorization: auth } })
                        .then(response => {
                            io.sockets.in(gameId).emit('receive question', response.data);
                        }).catch(error => {
                            console.log('Error retrieving question');
                            console.log(error);
                            io.sockets.in(gameId).emit('retryable error');
                        });
                } else {
                    console.log(`${gameId} is not found in the DB`);
                    io.sockets.in(gameId).emit('fatal error');
                    const socketsInGame = getSocketsInGame(gameId);
                    socketsInGame.forEach(s => {
                        s.leave(gameId, () => {
                            s.disconnect(true);
                        })
                    });
                }
            }
        });
    });


    socket.on('disconnect', function () {
        console.log('user disconnected');
        collection.findOne({ "gameId": socket.gameId }, (error, result) => {
            if (error) {
                console.log('Player not removed from game in MongoDB');
            } else {
                if (result) {
                    const remainingPlayers = result.players.filter(p => p.socketId !== socket.id);
                    collection.findOneAndUpdate(
                        { "gameId": socket.gameId },
                        { $set: { "players": remainingPlayers } },
                        { returnOriginal: false },
                        function (error, result) {
                            if (error) {
                                console.log('Player not removed from game in MongoDB');
                            } else {
                                io.sockets.in(socket.gameId).emit('player joined', result.value);
                            }
                        }
                    );
                } else {
                    console.log('Player not removed from game in MongoDB');
                }
            }
        });
    });
});
