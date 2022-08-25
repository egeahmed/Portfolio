
using APIs;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Handlers
{
    public class LoadingSceneHandler : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log("Loading scene loaded!");

            DatabaseAPI.InitializeDatabase();
            SceneManager.LoadScene(1);
        }
    }
}