using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Database;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance;

    [Header("Reference")]
    [SerializeField]
    public  DatabaseReference DBReference;

    //[SerializeField]
    //public FirebaseManager instance;

    
    private void Start()
    {
        //StartCoroutine(CheckAndFixDependencise());
        InitializeFirebase();
    }

    /*
    private IEnumerator CheckAndFixDependencise()
    {
        var checkAndFixTask = FirebaseApp.CheckAndFixDependenciesAsync();

        yield return new WaitUntil(predicate: () => checkAndFixTask.IsCompleted);

        var dependencyResult = checkAndFixTask.Result;

        if (dependencyResult == DependencyStatus.Available)
        {
            InitializeFirebase();
        }
        else
        {
            Debug.LogError($"COuld not resolve all Firebase dependencies: {dependencyResult}");
        }
    }
    

    */
    
    private void InitializeFirebase()
    {
        DBReference = FirebaseDatabase.GetInstance("https://sgpk-6eb69-default-rtdb.europe-west1.firebasedatabase.app/").RootReference;

    }
    


    public IEnumerator CreateUser()
    {
        string user_id = FirebaseManager.instance.user.UserId;
        PlayerData playerData = new PlayerData(user_id);

        string json = JsonUtility.ToJson(playerData);

        var DBTask = DBReference.Child("users").Child(user_id).SetRawJsonValueAsync(json);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
    }

    //todo: upload this when one game ends
    /*
    private IEnumerator UpdateGameList(string friend_id, GameRecords gameRecord) {

        
    }
    */


    private IEnumerator UpdateWaitingList()
    {
        var DBTask = DBReference.Child("lobby").SetValueAsync(FirebaseManager.instance.user.UserId);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);





    }



    




    private IEnumerator UpdateUser()
    {
        var DBTask = DBReference.Child("users").Child(FirebaseManager.instance.user.UserId).Child("username").SetValueAsync(FirebaseManager.instance.user.DisplayName);


        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);


        if(DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with{DBTask.Exception}");
        }
    }

    public void Save_test_button()
    {
        StartCoroutine(UpdateUser());
    }

}
