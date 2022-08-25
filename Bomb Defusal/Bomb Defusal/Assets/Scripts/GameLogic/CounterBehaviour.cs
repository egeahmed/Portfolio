using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CounterBehaviour : MonoBehaviour
{
    public static bool timeup = false;
    [SerializeField]
    private TextMesh counter;

    [SerializeField]
    private MeshRenderer renderer;

    [SerializeField]
    public float timeRemaining;

    private int updateFrequency = 1;

    // Start is called before the first frame update
    void Start()
    {
        DisplayTime();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= updateFrequency)
        {
            updateFrequency = Mathf.FloorToInt(Time.time) + 1;
            UpdatePerSecond();
        }
    }

    void UpdatePerSecond()
    {
        if (timeRemaining > 0)
        {
            if (timeRemaining == 5 || timeRemaining == 3 || timeRemaining == 1)
                StartCoroutine(BlinkCounter());
            timeRemaining--;
            DisplayTime();
        }
        else
        {
            counter.text = "00:00";
            Debug.Log("No time remaining...");
            timeup = true;
        }
    }

    void DisplayTime()
    {
        float minutes = Mathf.FloorToInt(timeRemaining / 60);
        float seconds = Mathf.FloorToInt(timeRemaining % 60);
        counter.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    IEnumerator BlinkCounter()
    {
        float time = Time.time;
        renderer.enabled = false;
        yield return new WaitForSeconds(0.1f);
        renderer.enabled = true;
    }
}
