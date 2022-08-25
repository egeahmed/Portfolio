using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using APIs;
using Firebase.Database;
using System;

public class BombProperties : MonoBehaviour
{
    private List<GameObject> buttons = new List<GameObject>();

    private KeyValuePair<DatabaseReference, EventHandler<ValueChangedEventArgs>> buttonListener;

    //private KeyValuePair<DatabaseReference, EventHandler<ValueChangedEventArgs>> rotationListener;

    private KeyValuePair<DatabaseReference, EventHandler<ValueChangedEventArgs>> loseListener;

    public bool exploded = false;

    
    [SerializeField]
    Transform bombTranform;

    public bool[] clickable_indexes;


    public void LoadButtons()
    {
        for (int i = 0; i < bombTranform.transform.childCount; i++)
        {
            Transform currentItem = bombTranform.transform.GetChild(i);
            if (currentItem.name.Contains("Button") || currentItem.name.Contains("Rope"))
            {
                buttons.Add(currentItem.gameObject);
            }
        }
        if (clickable_indexes.Length > Buttons.Count)
        {
            Debug.LogError("clickable Indexex bigger than numbers of buttons");
        }
        UpdateMaterial(true);
    }

    public List<GameObject> Buttons
    {
        // should not have a setter
        get { return buttons; }
    }

    public void UpdateMaterial(bool solver)
    {
        if (solver)
        {
            foreach (var l in buttons)
            {
                l.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", Color.grey);
            }
        }
        else
        {
            foreach (var l in buttons)
            {
                if (clickable_indexes[buttons.IndexOf(l)] == true)
                {
                    l.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", Color.green);
                }
                else
                {
                    l.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", Color.red);
                }
            }
        }
    }


    public void UpdataButton(string gameId,Action callback, Action<AggregateException> fallback)
    {
        DatabaseAPI.PostObject($"games/{gameId}/press", clickable_indexes,callback, fallback);
    }

    public void ListenForButton(string gameId, Action callback, Action<AggregateException> fallback)
    {
        buttonListener = DatabaseAPI.ListenForValueChanged($"games/{gameId}/press", args =>
        {
            if (!args.Snapshot.Exists) return;

            var pressArray = StringSerializationAPI.Deserialize(typeof(bool[]), args.Snapshot.GetRawJsonValue()) as bool[];

            clickable_indexes = pressArray;

            callback();

        }, fallback);
    }


    public void StopListenForButton() => DatabaseAPI.StopListeningForValueChanged(buttonListener);

    public void ListenForLose(string gameId, Action callback , Action<AggregateException> fallback)
    {
        loseListener = DatabaseAPI.ListenForValueChanged($"games/{gameId}/explode", args =>
        {
            if (!args.Snapshot.Exists) return;

            var explode = StringSerializationAPI.Deserialize(typeof(bool), args.Snapshot.GetRawJsonValue());

            exploded = (bool)explode;

            Debug.Log(exploded);

            if (exploded)
            {
                callback();
            }
            //callback();
        }, fallback);
    }

    public void StopListenForLose() => DatabaseAPI.StopListeningForValueChanged(loseListener);


    /*
    public void ListenForRotation(string gameId)
    {
        rotationListener = DatabaseAPI.ListenForValueChanged($"games/{}");
    }
    */

    public void UpdateExplosion(string gameId, Action<AggregateException> fallback)
    {
        DatabaseAPI.PostObject($"games/{gameId}/explode", exploded, ()=> { }, fallback);
    }

}
