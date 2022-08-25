
using UnityEngine;

namespace Managers
{
    public class MainManager : MonoBehaviour
    {
        public static MainManager Instance;

        public MatchmakingManager matchmakingManager;
        public GameManager gameManager;

        //public FirebaseManager firebaseManager;

        public string currentLocalPlayerId; 
        private void Awake() => Instance = this;

        private void Start()
        {
            matchmakingManager = GetComponent<MatchmakingManager>();
            gameManager = GetComponent<GameManager>();
        }


    }
}