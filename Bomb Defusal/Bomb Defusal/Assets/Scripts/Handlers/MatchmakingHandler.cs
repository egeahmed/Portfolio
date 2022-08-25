
using UnityEngine;

using UnityEngine.SceneManagement;


using Managers;
using System;
using UnityEngine.UI;

namespace Handlers
{

    public class MatchmakingHandler : MonoBehaviour
    {
        public GameObject searchingPanel;

        public GameObject foundPanel;

        public Dropdown RoleDropdown;


        private bool gameFound;

        private bool readyingUp;

        private string gameId;

        //this value represents the role for the game
        //if role == 0, then the player is a solver, and matching for instructor;
        //if role == 1, then the player is a instructor, and match for solver.
        private int role;

        



        //private void Start() => JoinQueue();

        private void Start()
        {
            RoleDropdown.value = 0;
        }


        public void setRole(int value)
        {
            role = value;
        }

        public void JoinQueue() =>
            MainManager.Instance.matchmakingManager.JoinQueue(MainManager.Instance.currentLocalPlayerId,role, gameId =>
            {
                this.gameId = gameId;
                gameFound = true;
            },
                Debug.Log);

        private void Update()
        {
            if (!gameFound || readyingUp) return;
            readyingUp = true;
            GameFound();
        }

        private void GameFound()
        {
            MainManager.Instance.gameManager.GetCurrentGameInfo(gameId, MainManager.Instance.currentLocalPlayerId,
                gameInfo =>
                {
                    Debug.Log("Game found. Ready-up!");
                    gameFound = true;
                    string[] ids = { gameInfo.solverId, gameInfo.insId};
                    MainManager.Instance.gameManager.ListenForAllPlayersReady(ids,
                        playerId => Debug.Log(playerId + " is ready!"), () =>
                        {
                            Debug.Log("All players are ready!");
                            SceneManager.LoadScene(2);
                        }, Debug.Log);
                }, Debug.Log);

            searchingPanel.SetActive(false);
            foundPanel.SetActive(true);
        }

        public void LeaveQueue()
        {
            if (gameFound) MainManager.Instance.gameManager.StopListeningForAllPlayersReady();
            else
                MainManager.Instance.matchmakingManager.LeaveQueue(MainManager.Instance.currentLocalPlayerId,
                    () => Debug.Log("Left queue successfully"), Debug.Log);

            //load new scene
            //SceneManager.LoadScene("MenuScene");
        }

        //todo: cancle the game
        //public void CancelGame(){}

        public void Ready() =>
            MainManager.Instance.gameManager.SetLocalPlayerReady(() => Debug.Log("You are now ready!"), Debug.Log);
    }
}

