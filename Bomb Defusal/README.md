# Social Gaming Praktikum

Repository for the social gaming Praktikum group 32


## assignment_1

## assignmnet_2

Using firebase features: [firebase authentication](https://firebase.google.com/docs/auth/unity/start), [firebase database](https://firebase.google.com/docs/database/unity/start), [firebase cloud functions](https://firebase.google.com/docs/functions) to implement a bomb-defusal game (sketches for the game idea can be found under assignment1/) with Unity. 

### Details with firebase

#### Authentication 

You can currently register via email, and login with the registered email of course.

#### Database

After login, you will be leading to a verification panel if you have not yet verified your email address. You can click "Later" to ignore the verification for now. 

Then you will find yourself at the matching panel, and click "matching" to start matching yourself up with other players to create a game. The matching system [^1] is not on the client (player) side in unity. Instead it is rather implemented in a file outside unity in Javascript (you can check code for cloud functions under /assignment2/cloud_function/functions/index.js). The basic idea for the matching system is to match the 2 players in the waiting list area in the database up, create game in the database according to the "Tie-strength" of the two matched players, and send the game info back to each plaey to start their games.


After a game ends, a record of this game will be uploaded to the database, and triggers a event for updating tie strength for both players according to this game results [^2].  

### Details with Game Logic

At beginning, ```gameLoop``` will automatically generate a Bomb depends on the difficulty getting from Database. The Bombs are in the ```Ressources/Bomb``` Directory.

#### BombBehaviour

Each bomb has it own script, which will automatically loads all the buttoms into a List. 

#### Game Loop

These stuffs will be handled in gameloop:
- InstantiateBomb
- Get input and send a raycast to the object
- Handle the behaviour to each object
- Upload Information to the Database
- Explode the bomb if something went wrong

### Voice Chat

The game features a voice chat functionality which automatically connects both players on entering a game so they can talk to each other.

#### Implementation

The voice chat functionality uses Vivox, a COTS solution for chat systems especially in gaming contexts.
The Vivox implementation is localized in the ```VoiceChat``` script which initializes connection and automatically connects the players of a game to their voice channel.

#### Starting the voice connection
The connection to the Vivox server is initialized as soon as the player joins a queue for matchmaking.
Once a game is found and both players ready up the voice channel is automatically set up and started between both of them.

### Future features

May move from firebase-database and firebase-functions into the deltaDNA platform for more fancy featrures to enhance more long-term social context.
Including a creator mode where individual players can create or defuse created bombs with special rewards.
Adding bombs with buttons which cannot be reached without flipping the bomb vertically, which would make for more challenging puzzles


## References

[^1]: [Multiplayer and Matchmaking system using Firebase Realtime Database in Unity!](https://www.youtube.com/watch?v=wBvWaTTxfmo)

[^2]: [Cloud Functions for Firebase Sample Library](https://github.com/firebase/functions-samples#environment)
