using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BombGyroRotation : MonoBehaviour
{

    public GameObject sample;
    //public Button button;


    private bool gyroEnabled;
    private Gyroscope gyro;
    private bool turnEnabled;



    // Start is called before the first frame update
    void Start()
    {
        gyroEnabled = EnableGyro();
        turnEnabled = true;
        sample.transform.position = transform.position;
        sample.transform.rotation = transform.rotation;
/*
        Button btn = button.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
  */
        }

/*
    void TaskOnClick()
    {
        Debug.Log("Button pressed");
        ToggleRotation();

    }
*/
    private bool EnableGyro()
    {
        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;

            return true;
        }
        return false;
    }

    private void ToggleRotation()
    {
        if (turnEnabled == false)
        {
            turnEnabled = true;
            Debug.Log("Enabled Turning");
        }

        else
        {
            turnEnabled = false;
            Debug.Log("Disabled Turning");
        }
    }



    // Update is called once per frame
    void Update()
    {

        if (gyroEnabled && turnEnabled)
        {
            /*
            if (Input.gyro.rotationRate.x > 5.0f)
            {

                StartCoroutine(Rotate(Vector3.right, 90, 1.5f));
            }

            else if (Input.gyro.rotationRate.x < -5.0f)
            {


                StartCoroutine(Rotate(Vector3.right, -90, 1.5f));
            }
            */

            if (Input.gyro.rotationRate.y > 5.0f)
            {


                StartCoroutine(Rotate(Vector3.up, 90, 1.5f));
            }


            else if (Input.gyro.rotationRate.y < -5.0f)
            {



                StartCoroutine(Rotate(Vector3.up, -90, 1.5f));
            }

        }

    }


    IEnumerator Rotate(Vector3 axis, float angle, float duration = 1.5f)
    {
        //button.enabled = false;
        ToggleRotation();
        Quaternion from = transform.rotation;
        sample.transform.Rotate(axis * angle, Space.World);
        Quaternion to = sample.transform.rotation;


        //Quaternion to = gameObject.transform.Rotate(axis, Space.World);
        //to *= Quaternion.Euler(axis * angle);
        //Quaternion to = transform.rotation;


        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;

            yield return null;
        }
        transform.rotation = to;
        ToggleRotation();
        //button.enabled = true;
    }


}
