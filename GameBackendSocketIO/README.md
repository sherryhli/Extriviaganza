# Game Backend

A Node.js application built with socket.io, Express.js, MongoDB, and Redis.

`server.js` facilitates the gameplay and real-time communication between players while `worker.js` runs as a background process (on a separate Heroku dyno) and populates a Redis cache of questions to speed up question retrieval.

## Server

The server handles the following events:

### `join game`

Parameters: `gameId` (uuid), `userId` (string), `existingGame` (boolean)<br>
When the backend receives this event, it adds the user identified by `userId` to the game identified by `gameId` if `existingGame = true` and the game actually exists in the DB. If `existingGame = true` but the game is not found in the DB, a `fatal error` is emitted to `userId`. If `existingGame = false`, a new game gets created and `userId` is added to it. An association is created between `userId` and that user's socket id. If successful, the server will emit `player joined` and return the updated game state to all players in `gameId`.

Errors: `fatal error`, `duplicate userId`
***

### `get question`

Parameters: `level` (string)<br>
After receiving this event, the server will attempt to retrieve a question from the cache. The set name is either supplied in the `level` parameter (`MiddleSchool`, `HighSchool`, `Collegiate` or `Trash`), or randomly chosen if `level` is null. (Note: level names are **not case-sensitive**). If the server fails to retrieve a question from the cache, then a GET request to the `/random` endpoint of [QbQuestionsAPI](https://github.com/sherryhli/Extriviaganza/tree/master/QbQuestionsAPI) is made with the appropriate query parameter. If there is a successful retrieval from either the cache or the API, the server will emit `receive question` and all players in the game associated the socket that invoked this event will receive the question.

Errors: `retryable error`, `fatal error`
***

### `buzz`

Parameters: `userId` (string)<br>
When the server receives this event, it emits `buzz acknowledged` to the player with `userId` and emits `other player buzzed` to all others players in `userId`'s game.

Errors: none
***

### `correct answer`

Parameters: `userId` (string), `power` (boolean)<br>
When the server receives this event, it adds `power ? 15 : 10` points to `userId`'s score. If successful, it emits `player answered correctly` and returns the updated game state to all players in `userId`'s game.

Errors: `retryable error`
***

### `incorrect answer`

Parameters: `userId` (string), `answer` (string), `power` (boolean)<br>
After receiving this event, the server applies a 5 point penalty to `userId`'s score if `power` is true (this means the player has interrupted) and emits `player answered incorrectly` to all players in `userId`'s game. If `power` is false, the server only emits `player answered incorrectly`.

Errors: `retryable error`
***

### `fatal error`

Parameters: none<br>
Typically emitted by the server if the only way to recover is for the client to reconnect. In most cases the server will force the socket to leave the room and close the connection after emitting this event.
***

### `retryable error`

Parameters: none<br>
This event is emitted by the server if an error occurred but the client should retry the operation.
***

### `duplicate userId`

Parameters: none<br>
This event is emitted by the server if a player tries to use a `userId` that's already being used by another player in their game.


## Worker

The worker continuously makes GET requests to the `/random` endpoint of [QbQuestionsAPI](https://github.com/sherryhli/Extriviaganza/tree/master/QbQuestionsAPI) in order to populate the Redis cache upfront with questions of all levels. The cache is implemented as a collection of sets, with questions of each level being in a separate set, e.g. `questions:MiddleSchool`, `questions:HighSchool`, etc. Sets were chosen over lists as they decrease the likelihood of requests made close in time returning the same question, and also limits memory usage as only unique questions are stored in each set.

The worker should be scaled down as we approach the 25 MB memory limit of free Heroku Redis (not likely anytime soon since there aren't enough questions in [QbQuestionsAPI](https://github.com/sherryhli/Extriviaganza/tree/master/QbQuestionsAPI)'s database). Although there is an LRU eviction policy, it will result in loss of an entire set of questions, which is not ideal.


## Deployment

The backend is deployed to Heroku at https://extriviaganza.herokuapp.com. There are currently two test endpoints: `/test1` and `/test2` that serve as very simple clients.

Commit any changes and then login to Heroku via the CLI:
```
heroku login
```

To deploy:
```
git subtree push --prefix GameBackendSocketIO heroku master
```

Note: since the game backend is in a subdirectory of my monorepo, we must use the above command instead of the usual `git push heroku master`.

Since we use socket.io, we must enable session affinity:
```
heroku features:enable http-session-affinity
```

To set any environment variables:
```
heroku config:set ENV_VAR_NAME=VALUE
```

To check status of dynos:
```
heroku ps
```

To run 1 web dyno:
```
heroku ps:scale web=1
```

Scale down worker dyno:
```
heroku ps:scale worker=0
```

To restart dyno:
```
heroku ps:restart
```
