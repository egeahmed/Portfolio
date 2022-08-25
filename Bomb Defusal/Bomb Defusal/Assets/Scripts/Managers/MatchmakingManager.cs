using System;
using System.Collections.Generic;
using APIs;
using Firebase.Database;
using UnityEngine;

namespace Managers
{
    public class MatchmakingManager : MonoBehaviour
    {
        private KeyValuePair<DatabaseReference, EventHandler<ValueChangedEventArgs>> queueListener;

        public void JoinQueue(string playerId, int role, Action<string> onGameFound, Action<AggregateException> fallback) =>
            DatabaseAPI.PostObject($"matchmaking/{playerId}", role,
                () => queueListener = DatabaseAPI.ListenForValueChanged($"matchmaking/{playerId}",
                    args =>
                    {
                        var gameId =
                            StringSerializationAPI.Deserialize(typeof(string), args.Snapshot.GetRawJsonValue()) as
                                string;
                        if (gameId == "0" || gameId == "1") return;
                        LeaveQueue(playerId, () => onGameFound(
                            gameId), fallback);
                    }, fallback), fallback);

        public void LeaveQueue(string playerId, Action callback, Action<AggregateException> fallback)
        {
            DatabaseAPI.StopListeningForValueChanged(queueListener);
            DatabaseAPI.PostJSON($"matchmaking/{playerId}", "null", callback, fallback);
        }
    }
}