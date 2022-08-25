using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;


using TMPro;
using Managers;
using Handlers;

using APIs;
using System;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager instance;

    [Header("Firebase")]
    public FirebaseAuth auth;
    public FirebaseUser user;

    //[SerializeField]
    //public GameObject databaseManager;
    //public static DatabaseManager instance_databse;
    [Space(5f)]

    [Header("Login Regerences")]
    [SerializeField]
    private TMP_InputField loginEmial;
    [SerializeField]
    private TMP_InputField loginPassword;
    [SerializeField]
    private TMP_Text loginOutputText;
    [Space(5f)]

    [Header("Register Regerences")]
    [SerializeField]
    private TMP_InputField registerUserName;
    [SerializeField]
    private TMP_InputField registerEmial;
    [SerializeField]
    private TMP_InputField registerPassword;
    [SerializeField]
    private TMP_InputField registerConfirmPassword;
    [SerializeField]
    private TMP_Text registerOutputText;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(instance.gameObject);
            instance = this;
        }

        /*
        //check Firebase dependency
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(checkTask => {
            var dependencyStatus = checkTask.Result;

            if(dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            }

        });
        */

    }

    private void Start()
    {
        StartCoroutine(CheckAndFixDependencise());

        //databaseManager = GetComponent<DatabaseManager>();
    }


    private IEnumerator CheckAndFixDependencise()
    {
        var checkAndFixTask = FirebaseApp.CheckAndFixDependenciesAsync();

        yield return new WaitUntil(predicate: () => checkAndFixTask.IsCompleted);

        var dependencyResult = checkAndFixTask.Result;

        if(dependencyResult == DependencyStatus.Available)
        {
            InitializeFirebase();
        }
        else
        {
            Debug.LogError($"COuld not resolve all Firebase dependencies: {dependencyResult}");
        }
    }


    private void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        StartCoroutine(CheckAutoLogin());

        auth.StateChanged += AuthStateChanged;

        AuthStateChanged(this, null);


        //instance_databse.DBReference = FirebaseDatabase.DefaultInstance.RootReference;

        //instance_databse.DBReference = FirebaseDatabase.GetInstance("https://sgpk-6eb69-default-rtdb.europe-west1.firebasedatabase.app/").RootReference;

    }

    private IEnumerator CheckAutoLogin()
    {
        yield return new WaitForEndOfFrame();

        if(user != null)
        {
            var reloadTask = user.ReloadAsync();

            yield return new WaitUntil(predicate: () => reloadTask.IsCompleted);

            MainManager.Instance.currentLocalPlayerId = user.UserId;

            AutoLogin();
        }
        else
        {
            AuthUIHandler.instance.LoginScreen();
        }
    }

    private void AutoLogin()
    {
        if(user != null)
        {
            CheckUserProfile();

            //todo:
            if (user.IsEmailVerified)
            {

                AuthUIHandler.instance.TestScreen();
                //GameManeger.instance.ChangeScene(1);
            }
            else
            {
                AuthUIHandler.instance.SendScreen();
                //StartCoroutine(SendVerificationEmail());
            }

            //AuthUIManager.instance.TestScreen();

            //GameManeger.instance.ChangeScene(1);
        }
        else
        {
            AuthUIHandler.instance.LoginScreen();
        }
    }



    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if(auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

            if (!signedIn && user != null)
            {
                Debug.Log("Signed Out");
            }

            user = auth.CurrentUser;

            if (signedIn)
            {
                Debug.Log($"Signed In: {user.DisplayName}");
            }
        }
    }


    public void ClearLoginField()
    {
        loginEmial.text = "";
        loginPassword.text = "";
        loginOutputText.text = "";

    }

    public void ClearRegisterField()
    {
        registerEmial.text = "";
        registerPassword.text = "";
        registerConfirmPassword.text = "";

    }


    public void ClearOutputs()
    {
        loginOutputText.text = "";
        registerOutputText.text = "";
    }

    public void LoginButton()
    {
        StartCoroutine(LoginLogic(loginEmial.text, loginPassword.text));
        //ClearLoginField();
    }

    public void RegisterButton()
    {
        StartCoroutine(RegisterLogic(registerUserName.text, registerEmial.text, registerPassword.text, registerConfirmPassword.text));
    }

    public void SignOutButton()
    {
        auth.SignOut();

        AuthUIHandler.instance.LoginScreen();
        ClearRegisterField();
        ClearLoginField();
    }

    public void SendButton()
    {
        StartCoroutine(SendVerificationEmail());
    }


    private IEnumerator LoginLogic(string _email, string _password)
    {
        Credential credential = EmailAuthProvider.GetCredential(_email, _password);

        var loginTask = auth.SignInWithCredentialAsync(credential);

        yield return new WaitUntil(predicate: () => loginTask.IsCompleted);

        if(loginTask.Exception != null)
        {
            FirebaseException firebaseException = (FirebaseException)loginTask.Exception.GetBaseException();

            AuthError error = (AuthError)firebaseException.ErrorCode;

            string output = "Unknown Error, Please try again after a few seconds!";

            switch (error)
            {
                //todo here:
                case AuthError.MissingEmail:
                    output = "Please Enter Your Emial";
                    break;
                case AuthError.InvalidEmail:
                    output = "Invalid Email or password";
                    break;
                case AuthError.WrongPassword:
                    output = "Invalie Email or password";
                    break;
                case AuthError.UserNotFound:
                    output = "Account Does NOt Exist";
                    break;
                //todo:

            }

            loginOutputText.text = output;
        }
        else
        {

            MainManager.Instance.currentLocalPlayerId = user.UserId;

            CheckUserProfile();

            if (user.IsEmailVerified)
            {
                yield return new WaitForSeconds(0.5f);
                AuthUIHandler.instance.TestScreen();
                //GameManeger.instance.ChangeScene(1);
            }
            else
            {
                //TODO: Send Verification Email
                AuthUIHandler.instance.SendScreen();
                //StartCoroutine(SendVerificationEmail());
            }
        }
    }


    private void CheckUserProfile()
    {
        string userId = user.UserId;

        DatabaseAPI.CheckIfNodeExists($"users/{userId}", (exist) =>
        {
            if (exist)
            {
                Debug.Log("user file created");
                return;
            }
            else
            {
                Debug.Log("user file not created!");
                PlayerData playerData = new PlayerData(user.DisplayName);
                DatabaseAPI.PostObject($"users/{userId}", playerData, () =>
                {
                    DatabaseAPI.CheckIfNodeExists($"users/{userId}", (exist) =>
                    {
                        if (exist)
                        {
                            Debug.Log("manage to create!");
                        }
                        else
                        {
                            Debug.Log("fail to create!");
                        }
                    }, Debug.Log);
                }, Debug.Log);
            }
        }, Debug.Log);
    }

    private IEnumerator RegisterLogic(string _username, string _email, string _password, string _cofirmpassword)
    {
        if(_username == null)
        {
            registerOutputText.text = "Please Enter a User Name!";

        }else if (_password != _cofirmpassword)
        {
            registerOutputText.text = ("Passwords are not the same! Please enter again!");
        }
        else
        {
            var registerTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);

            yield return new WaitUntil(predicate: () => registerTask.IsCompleted);

            if (registerTask.Exception != null)
            {
                FirebaseException firebaseException = (FirebaseException)registerTask.Exception.GetBaseException();

                AuthError error = (AuthError)firebaseException.ErrorCode;

                string output = "Unknown Error, Please try again after a few seconds!";

                switch (error)
                {
                    //todo here:
                    case AuthError.EmailAlreadyInUse:
                        output = "Email already registered!";
                        break;
                    case AuthError.InvalidEmail:
                        output = "Invalid Email or password";
                        break;
                    case AuthError.WeakPassword:
                        output = "Your password is too weak!";
                        break;
                    case AuthError.MissingEmail:
                        output = "Please enter your Email!";
                        break;
                        //todo:

                }

                registerOutputText.text = output;
            }
            else
            {
                UserProfile profile = new UserProfile
                {
                    DisplayName = _username,

                    //todo here:
                };

                var defaultUserTask = user.UpdateUserProfileAsync(profile);

                yield return new WaitUntil(predicate: () => defaultUserTask.IsCompleted);


                if (defaultUserTask.Exception != null)
                {
                    user.DeleteAsync();

                    FirebaseException firebaseException = (FirebaseException)defaultUserTask.Exception.GetBaseException();

                    AuthError error = (AuthError)firebaseException.ErrorCode;

                    string output = "Unknown Error, Please try again after a few seconds!";

                    switch (error)
                    {
                        //todo here:
                        case AuthError.Cancelled:
                            output = "Update User Cancelled!";
                            break;
                        case AuthError.SessionExpired:
                            output = "Session Expired";
                            break;
                            //todo:

                    }

                    registerOutputText.text = output;
                }
                else
                {
                    CheckUserProfile();

                    //after registeration successfully, sent a verification email
                    //Debug.Log($"Firebase User Created Successfully: {user.DisplayName} ({user.UserId})");

                    //databaseManager.GetComponent<DatabaseManager>().StartCoroutine(databaseManager.GetComponent<DatabaseManager>().CreateUser());
                    //todo:add user into database


                    AuthUIHandler.instance.SendScreen();
                    //StartCoroutine(SendVerificationEmail());
                }
            }

        }
    }

    public IEnumerator SendVerificationEmail()
    {
        if(user != null)
        {
            var emailTask = user.SendEmailVerificationAsync();

            yield return new WaitUntil(predicate: () => emailTask.IsCompleted);

            if(emailTask.Exception != null)
            {
                FirebaseException firebaseException = (FirebaseException)emailTask.Exception.GetBaseException();

                AuthError error = (AuthError)firebaseException.ErrorCode;

                string output = "Unknown Error, Try Again!";

                switch (error)
                {
                    case AuthError.Cancelled:
                        output = "Verification Task was Cancelled";
                        break;
                    case AuthError.InvalidRecipientEmail:
                        output = "Invalid Email";
                        break;
                    case AuthError.TooManyRequests:
                        output = "Too many Verification Requests";
                        break;
                }

                AuthUIHandler.instance.AwaitVerification(false, user.Email, output);
            }
            else
            {
                AuthUIHandler.instance.AwaitVerification(true, user.Email, null);
                Debug.Log("Email Sent Successfully!");
            }
        }
    }

}
