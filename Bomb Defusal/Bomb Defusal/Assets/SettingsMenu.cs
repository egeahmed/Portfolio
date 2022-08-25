using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;


public class PauseMenu : MonoBehaviour
{

    void Start()
    {

    }

    /*
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }

        }
    }

    void Resume()
    {
        pauseMenuUI.setActive(false);
        Timeout.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.setActive(true);
        Timeout.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMenu() {
        SceneManager.LoadScene("Auth_Scene");
    }

    public void QuitGame() {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
    */
}
      