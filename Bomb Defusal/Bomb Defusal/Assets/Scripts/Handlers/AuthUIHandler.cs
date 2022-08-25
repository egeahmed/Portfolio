
using UnityEngine;
using TMPro;
using System.ComponentModel;

namespace Handlers
{
    public class AuthUIHandler : MonoBehaviour
    {
        public static AuthUIHandler instance;

        [Header("References")]

        [SerializeField]
        private GameObject loginUI;
        [SerializeField]
        private GameObject registerUI;
        [SerializeField]
        private GameObject verifyEmailUI;
        [SerializeField]
        private TMP_Text verityEmailText;
        [SerializeField]
        private GameObject sendVerifyUI;
        [SerializeField]
        private GameObject testUI;
        [SerializeField]
        private GameObject searchingPanel;



        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void ClearUI()
        {
            FirebaseManager.instance.ClearOutputs();
            loginUI.SetActive(false);
            registerUI.SetActive(false);
            verifyEmailUI.SetActive(false);
            //checkingForAccountUI.SetActive(false);
            testUI.SetActive(false);
            sendVerifyUI.SetActive(false);
            searchingPanel.SetActive(false);

        }

        public void LoginScreen()
        {
            ClearUI();
            loginUI.SetActive(true);
        }

        public void RegisterScreen()
        {
            ClearUI();
            registerUI.SetActive(true);
        }


        public void TestScreen()
        {
            ClearUI();
            testUI.SetActive(true);
        }

        public void SendScreen()
        {
            ClearUI();
            sendVerifyUI.SetActive(true);
        }

        public void SearchingScreen()
        {
            ClearUI();
            searchingPanel.SetActive(true);
        }


        public void Cancel()
        {
            ClearUI();

            testUI.SetActive(true);
        }

        //name will be changed
        public void AwaitVerification(bool _emailSent, string _email, string _output)
        {
            ClearUI();
            //verifyEmailUI.SetActive(true);
            if (_emailSent)
            {
                verityEmailText.text = $"Sent Emial!\n Please verify {_email}";

            }
            else
            {
                verityEmailText.text = $"Email Not Sent: {_output}\n Please Verify {_email}";
            }

            testUI.SetActive(true);

        }

    }

}


