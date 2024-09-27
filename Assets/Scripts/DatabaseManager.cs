using UnityEngine;
using Firebase;
using Firebase.Database;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class DatabaseManager : MonoBehaviour
{
    public InputField Name;
    public InputField Value;

    public Text nameText, valueText;
    public Text distanceText;

    private DatabaseReference dbReference;

    private string userID;

    private void Start()
    {
        userID = SystemInfo.deviceUniqueIdentifier;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    private void Update()
    {
        StartCoroutine(GetData((int distance) => {
            distanceText.text = "Khoảng cách: " + distance.ToString();
        }));

    }

    public void CreateUser()
    {
        User newUser = new User(Name.text, int.Parse(Value.text));
        string json = JsonUtility.ToJson(newUser);

        dbReference.Child("Users").Child(userID).SetRawJsonValueAsync(json);
    }
    public IEnumerator GetName(Action<string> onCallback)
    {
        var userNameData = dbReference.Child("Users").Child(userID).Child("name").GetValueAsync();

        yield return new WaitUntil(predicate: () => userNameData.IsCompleted);
        if(userNameData != null)
        {
            DataSnapshot snapshot = userNameData.Result;
            onCallback.Invoke(snapshot.Value.ToString());
        }
    }
    public IEnumerator GetValue(Action<int> onCallback)
    {
        var userValueData = dbReference.Child("Users").Child(userID).Child("value").GetValueAsync();

        yield return new WaitUntil(predicate: () => userValueData.IsCompleted);
        if (userValueData != null)
        {
            DataSnapshot snapshot = userValueData.Result;
            onCallback.Invoke(int.Parse(snapshot.Value.ToString()));
        }
    }
    public void GetUserInfo()
    {
        StartCoroutine(GetName((string name) => {
            nameText.text = "Value: " + name;

        }));
        StartCoroutine(GetValue((int value) => {
            valueText.text = "Value: " + value.ToString();

        }));
    }
    public void UpdateUserInfo()
    {
        dbReference.Child("User").Child(userID).Child("name").SetValueAsync(Name.text);
        dbReference.Child("User").Child(userID).Child("value").SetValueAsync(Value.text);
    }

    //My code
    public IEnumerator GetData(Action<int> onCallback)
    {
        var distance = dbReference.Child("Khoang cach").GetValueAsync();

        yield return new WaitUntil(predicate: () => distance.IsCompleted);
        if (distance != null)
        {
            DataSnapshot snapshot = distance.Result;
            onCallback.Invoke(int.Parse(snapshot.Value.ToString()));
        }
    }
}
