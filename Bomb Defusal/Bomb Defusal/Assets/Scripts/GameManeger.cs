
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManeger : MonoBehaviour
{

    public static GameManeger instance;

    private void Awake()
    {

        DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = this;
        }else if (instance != this)
        {
            Destroy(gameObject);

        }
    }


    public void ChangeScene(int _sceneIndex)
    {
        SceneManager.LoadSceneAsync(_sceneIndex);
    }

}
