if (process.env.NODE_ENV !== 'production') {
    require('dotenv').config();
}

const redis = require('redis');
const REDIS_URL = process.env.REDIS_URL || 'redis://127.0.0.1:6379';
const redisClient = redis.createClient(REDIS_URL);

const axios = require('axios');
const jwt = require('jsonwebtoken');

const QBQUESTIONS_API_BASE_URL = 'http://qbquestionsapi.azurewebsites.net/api';
const QBQUESTIONS_API_CREDENTIALS = {
    'username': process.env.QBQUESTIONS_API_WORKER_USERNAME,
    'password': process.env.QBQUESTIONS_API_WORKER_PASSWORD
};
const GET_QUESTION_INTERVAL_MS = 1000;
const REDIS_TOKEN_KEY = 'token';
const REDIS_QUESTIONS_KEY = 'questions';


redisClient.on('connect', () => {
    console.log('Connected to Redis server');
});


function getToken() {
    return new Promise((resolve, reject) => {
        redisClient.get(REDIS_TOKEN_KEY, (err, reply) => {
            if (err) {
                console.log('Error fetching token from Redis');

                axios.post(`${QBQUESTIONS_API_BASE_URL}/authenticate`, QBQUESTIONS_API_CREDENTIALS)
                    .then(response => {
                        redisClient.set(REDIS_TOKEN_KEY, response.data);
                        resolve(response.data);
                    }).catch(err => {
                        console.log('Error fetching token from API');
                        reject(err);
                    });
            } else {
                const decodedToken = jwt.decode(reply);

                if (decodedToken && decodedToken.exp > Math.round((new Date()).getTime() / 1000)) {
                    resolve(reply);
                } else {
                    axios.post(`${QBQUESTIONS_API_BASE_URL}/authenticate`, QBQUESTIONS_API_CREDENTIALS)
                        .then(response => {
                            redisClient.set(REDIS_TOKEN_KEY, response.data);
                            resolve(response.data);
                        }).catch(err => {
                            console.log('Error fetching token from API');
                            reject(err);
                        });
                }
            }
        });
    });
}


setInterval(() => {
    getToken()
        .then(token => {
            const auth = `Bearer ${token}`;
            axios.get(`${QBQUESTIONS_API_BASE_URL}/qbquestions/random`, { headers: { Authorization: auth } })
                .then(response => {
                    const question = response.data;
                    const levelSplitAtSpace = question.level.split(' ');
                    const level = levelSplitAtSpace[0] + levelSplitAtSpace[1];
                    const key = `${REDIS_QUESTIONS_KEY}:${level}`;
                    redisClient.sadd(key, JSON.stringify(question));
                }).catch(error => {
                    console.log('Error retrieving question from API');
                    console.log(error);
                });
        })
        .catch(err => {
            console.log('Error getting token');
        });
}, GET_QUESTION_INTERVAL_MS);
