using UnityEngine;
using Firebase.Database;
using Firebase;
using Firebase.Auth;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class FirebaseManager : MonoBehaviour
{
    [Header("Device")]
    public JsonSaveSystem jsonSaveSystem;

    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;
    public DatabaseReference dbReference;

    [Header("Login")]
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TextMeshProUGUI warningLoginText;

    [Header("UserData")]
    [Header("Slider")]
    [SerializeField] private Slider waterLevelSlider;
    [SerializeField] private Slider temperatureSlider;
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private TextMeshProUGUI waterLevelText;
    [SerializeField] private TextMeshProUGUI temperatureText;
    [SerializeField] private TextMeshProUGUI feedTimeInDayText;
    [SerializeField] private TextMeshProUGUI amountText;
    [Header("Toggle")]
    public Toggle pumpToggle;
    public Toggle lightToggle;
    public Toggle modeToggle;
    [Header("Panel")]
    [SerializeField] private GameObject lightManualPanel;
    [SerializeField] private GameObject lightAutoPanel;
    [SerializeField] private GameObject feedManualPanel;
    [SerializeField] private GameObject feedAutoPanel;
    [SerializeField] private GameObject pumpManualPanel;
    [SerializeField] private GameObject pumpAutoPanel;

    private NoficationManager noficationManager;
    private UIManager uiManager;
    private SoundManager soundManager;
    private ScheduleManager scheduleManager;

    private int maxAmount = 5;
    private bool canFeed = true;
    private bool loadingData = false;
    private bool alreadyFeed;
    bool start = true;

    private void Awake()
    {
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avaiable Intitialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.Log("Could not resolve all Firebase dependencies " + dependencyStatus);
            }
        });

        noficationManager = GetComponent<NoficationManager>();
        uiManager = GetComponent<UIManager>();
        soundManager = GetComponent<SoundManager>();
        scheduleManager = GetComponent<ScheduleManager>();
        jsonSaveSystem = GetComponent<JsonSaveSystem>();
        
    }
    private void Start()
    {
        waterLevelSlider.maxValue = 230f;
        temperatureSlider.maxValue = 50f;
        PrepareInfoAccount();
    }
    private void InitializeFirebase()
    {
        //Set the  authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;      
    }
    private void PrepareInfoAccount()
    {
        jsonSaveSystem.LoadFromJson();

        emailField.text = jsonSaveSystem.data.email;
        passwordField.text = jsonSaveSystem.data.password;
    }
    public void Login()
    {
        StartCoroutine(Login(emailField.text, passwordField.text));   
    }
    public void SignOut()
    {
        loadingData = false;
        auth.SignOut();
        uiManager.LoginScreen();
        warningLoginText.text = "";
    }
    private IEnumerator Login(string email, string password)
    {
        //Call the Firebase auth signin fuction passing email and password
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(email, password);
        //Loading screen
        uiManager.LoadingScreen(true);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //If there errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "User Not Found";
                    break;
            }
            uiManager.LoadingScreen(false);
            warningLoginText.text = message;
            soundManager.PlayErrorSound();
        }
        else
        {
            //User is now logged in, now we gonna save info account
            if (emailField.text != null && passwordField.text != null)
            {
                jsonSaveSystem.data.SaveAccount(emailField.text, passwordField.text);
            }
            //Now get the result
            user = LoginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.Email);       
            warningLoginText.text = "Logged In";
            
            StartCoroutine(LoadUserData());     
        }
    }

    //Data User
    public void CreateInitiateData()
    {
        dbReference.Child("Users").Child(user.UserId).Child("Username").SetValueAsync("Dong Nguyen");
        dbReference.Child("Users").Child(user.UserId).Child("Mode").SetValueAsync("MANUAL");
        dbReference.Child("Users").Child(user.UserId).Child("FeedState").SetValueAsync("OFF");
        dbReference.Child("Users").Child(user.UserId).Child("FeedTimer").SetValueAsync("null");
        dbReference.Child("Users").Child(user.UserId).Child("FeedTime").SetValueAsync("0");
        dbReference.Child("Users").Child(user.UserId).Child("Distance").SetValueAsync("0");
        dbReference.Child("Users").Child(user.UserId).Child("Temperature").SetValueAsync("0");
        dbReference.Child("Users").Child(user.UserId).Child("LightState").SetValueAsync("OFF");
        dbReference.Child("Users").Child(user.UserId).Child("PumpState").SetValueAsync("OFF");
        dbReference.Child("Users").Child(user.UserId).Child("Amount").SetValueAsync("1");

        pumpToggle.isOn = false;
        lightToggle.isOn = false;
        modeToggle.isOn = false;

        noficationManager.ResetProgress();
    }
    private IEnumerator LoadUserData()
    {
        loadingData = true;
        LoadData();
        yield return new WaitForSeconds(2.5f);
        start = true;
        soundManager.PlayLoginSound();
        uiManager.EnterMainScreen();
        yield return new WaitForSeconds(2f);
        start = false;
    }
    private void LoadData()
    {
        StartCoroutine(GetMode((String state) =>
        {
            if (state == "AUTOMATIC") modeToggle.isOn = true;
            if (state == "MANUAL") modeToggle.isOn = false;
            SwitchPanel(modeToggle.isOn);
        }));
        StartCoroutine(GetLightState((String state) =>
        {
            if (state == "OFF") lightToggle.isOn = false;
            if (state == "ON") lightToggle.isOn = true;
        }));
        StartCoroutine(GetPumpState((String state) =>
        {
            if (state == "OFF") pumpToggle.isOn = false;
            if (state == "ON") pumpToggle.isOn = true;
        }));
        StartCoroutine(GetUsername((String state) =>
        {
            usernameText.text = state.ToString();
        }));

        scheduleManager.ReloadTimeBar();
    }
    private void Update()
    {
        jsonSaveSystem.SaveToJson();
        if (loadingData == true)
        {
            UpdateData();
            UpdateDataSensor();
        }
    }
    // GetData from Firebase
    private IEnumerator GetUsername(Action<string> onCallback)
    {
        var username = dbReference.Child("Users").Child(user.UserId).Child("Username").GetValueAsync();

        yield return new WaitUntil(predicate: () => username.IsCompleted);
        if (username != null)
        {
            DataSnapshot snapshot = username.Result;
            onCallback.Invoke(snapshot.Value.ToString());
        }
    }
    private IEnumerator GetMode(Action<string> onCallback)
    {
        var mode = dbReference.Child("Users").Child(user.UserId)
            .Child("Mode").GetValueAsync();

        yield return new WaitUntil(predicate: () => mode.IsCompleted);
        if (mode != null)
        {
            DataSnapshot snapshot = mode.Result;
            onCallback.Invoke(snapshot.Value.ToString());
        }
    }
    private IEnumerator GetDistanceValueSensor(Action<int> onCallback)
    {
        var waterLv = dbReference.Child("Users").Child(user.UserId)
            .Child("Distance").GetValueAsync();

        yield return new WaitUntil(predicate: () => waterLv.IsCompleted);
        if (waterLv != null)
        {
            DataSnapshot snapshot = waterLv.Result;
            onCallback.Invoke(int.Parse(snapshot.Value.ToString()));
        }
    }
    private IEnumerator GetTemperatureValueSensor(Action<float> onCallback)
    {
        var temp = dbReference.Child("Users").Child(user.UserId)
            .Child("Temperature").GetValueAsync();

        yield return new WaitUntil(predicate: () => temp.IsCompleted);
        if (temp != null)
        {
            DataSnapshot snapshot = temp.Result;
            onCallback.Invoke(float.Parse(snapshot.Value.ToString()));
        }
    }
    private IEnumerator GetAmount(Action<int> onCallback)
    {
        var amount = dbReference.Child("Users").Child(user.UserId)
            .Child("Amount").GetValueAsync();

        yield return new WaitUntil(predicate: () => amount.IsCompleted);
        if (amount != null)
        {
            DataSnapshot snapshot = amount.Result;
            onCallback.Invoke(int.Parse(snapshot.Value.ToString()));
        }
    }
    private IEnumerator GetFeedTime(Action<int> onCallback)
    {
        var feedTime = dbReference.Child("Users").Child(user.UserId)
            .Child("FeedTime").GetValueAsync();

        yield return new WaitUntil(predicate: () => feedTime.IsCompleted);
        if (feedTime != null)
        {
            DataSnapshot snapshot = feedTime.Result;
            onCallback.Invoke(int.Parse(snapshot.Value.ToString()));
        }
    }
    private IEnumerator GetFeedState(Action<string> onCallback)
    {
        var feedState = dbReference.Child("Users").Child(user.UserId)
            .Child("FeedState").GetValueAsync();

        yield return new WaitUntil(predicate: () => feedState.IsCompleted);
        if (feedState != null)
        {
            DataSnapshot snapshot = feedState.Result;
            onCallback.Invoke(snapshot.Value.ToString());
        }
    }
    private IEnumerator GetLightState(Action<string> onCallback)
    {
        var lightState = dbReference.Child("Users").Child(user.UserId)
            .Child("LightState").GetValueAsync();

        yield return new WaitUntil(predicate: () => lightState.IsCompleted);
        if (lightState != null)
        {
            DataSnapshot snapshot = lightState.Result;
            onCallback.Invoke(snapshot.Value.ToString());
        }
    }
    private IEnumerator GetPumpState(Action<string> onCallback)
    {
        var pumpState = dbReference.Child("Users").Child(user.UserId)
            .Child("PumpState").GetValueAsync();

        yield return new WaitUntil(predicate: () => pumpState.IsCompleted);
        if (pumpState != null)
        {
            DataSnapshot snapshot = pumpState.Result;
            onCallback.Invoke(snapshot.Value.ToString());
        }
    }
    //Update data
    private void UpdateData()
    {
        //Feed state
        StartCoroutine(GetFeedState((String state) =>
        {
            if (state == "OFF") canFeed = true;
            if (state == "ON") canFeed = false;
        }));
        // Amount feed
        StartCoroutine(GetAmount((int amount) =>
        {
            amountText.text = amount.ToString();
        }));
        // Feed Time feed
        StartCoroutine(GetFeedTime((int amount) =>
        {
            feedTimeInDayText.text = amount.ToString();
        }));
    }
    private void UpdateDataSensor()
    {
        StartCoroutine(GetDistanceValueSensor((int waterLevel) =>
        {
            int vl = 24 - waterLevel;
            waterLevelText.text = vl.ToString() + "cm";
            waterLevelSlider.maxValue = 50;
            waterLevelSlider.value = vl;
        }));
        StartCoroutine(GetTemperatureValueSensor((float temp) =>
        {
            temperatureText.text = temp.ToString() + "°C";
            temperatureSlider.maxValue = 50;
            temperatureSlider.value = temp;
        }));
        
    }
    //Feed timer
    public void UpdateFeedTimeBar(string timer, int index)
    {
        dbReference.Child("Users").Child(user.UserId).Child("FeedTimer").Child(index.ToString()).SetValueAsync(timer);
    }
    public void RemoveSetFeedTimer()
    {
        StartCoroutine(RemoveAllSetFeedTimer());
    }
    private IEnumerator RemoveAllSetFeedTimer()
    {
        yield return new WaitForSeconds(1f);
        dbReference.Child("Users").Child(user.UserId).Child("FeedTimer").RemoveValueAsync();
    }
    //Light timer
    public void UpdateLightTimeBar(string timer)
    {
        dbReference.Child("Users").Child(user.UserId).Child("LightTimerOn").SetValueAsync(timer);
    }
    public void UpdateLightOffTimeBar(string timer)
    {
        dbReference.Child("Users").Child(user.UserId).Child("LightTimerOff").SetValueAsync(timer);
    }
    public void RemoveSetLightTimer()
    {
        StartCoroutine(RemoveAllSetLightTimer());
    }
    private IEnumerator RemoveAllSetLightTimer()
    {
        yield return new WaitForSeconds(1f);
        dbReference.Child("Users").Child(user.UserId).Child("LightTimer").RemoveValueAsync();
    }
    //
    private void SwitchPanel(bool state)
    {
        lightAutoPanel.SetActive(state);
        lightManualPanel.SetActive(!state);
        feedAutoPanel.SetActive(state);
        feedManualPanel.SetActive(!state);
        pumpAutoPanel.SetActive(state);
        pumpManualPanel.SetActive(!state);
    }
    //Action
    public void SwitchMode()
    {
        if (!start)
        {
            if (!modeToggle.isOn)
            {
                dbReference.Child("Users").Child(user.UserId).Child("Mode").SetValueAsync("MANUAL");
                dbReference.Child("Users").Child(user.UserId).Child("FeedState").SetValueAsync("OFF");
                noficationManager.ManualMode();
            }
            if (modeToggle.isOn)
            {
                dbReference.Child("Users").Child(user.UserId).Child("Mode").SetValueAsync("AUTOMATIC");
                dbReference.Child("Users").Child(user.UserId).Child("FeedState").SetValueAsync("ON");
                noficationManager.AutoMode();
            }
            SwitchPanel(modeToggle.isOn);
            soundManager.PlaySwitchButtonSound();
        }
    }
    public void FeedButton()
    {
        soundManager.PlayFeedButtonSound();

        //Manual mode
        if (!modeToggle.isOn)
        {
            if (canFeed)
            {
                dbReference.Child("Users").Child(user.UserId).Child("FeedState").SetValueAsync("ON");
                noficationManager.Success();
            }
            if (!canFeed)
            {
                noficationManager.Alert();
            }     
        }
        //Auto mode
        if (modeToggle.isOn)
        {
            noficationManager.AutoMode();
        }
    }
    public void Feeding()
    {
        if (!alreadyFeed)
        {
            alreadyFeed = true;
            dbReference.Child("Users").Child(user.UserId).Child("FeedState").SetValueAsync("ON");
            noficationManager.Success();
        }
    }
    public void Light()
    {
        if (!start)
        {
            if (!lightToggle.isOn)
            {
                dbReference.Child("Users").Child(user.UserId).Child("LightState").SetValueAsync("OFF");
            }
            if (lightToggle.isOn)
            {
                dbReference.Child("Users").Child(user.UserId).Child("LightState").SetValueAsync("ON");
            }
            soundManager.PlaySwitchButtonSound();
        }
    }
    public void Pump()
    {
        if (!start)
        {
            if (!pumpToggle.isOn)
            {
                dbReference.Child("Users").Child(user.UserId).Child("PumpState").SetValueAsync("OFF");
            }
            if (pumpToggle.isOn)
            {
                dbReference.Child("Users").Child(user.UserId).Child("PumpState").SetValueAsync("ON");
            }
            soundManager.PlaySwitchButtonSound();
        }
    }
    public void Plus()
    {
        StartCoroutine(GetAmount((int amount) =>
        {
            amount += 1; ;
            if (amount >= maxAmount)
            {
                amount = maxAmount;
            }
            dbReference.Child("Users").Child(user.UserId).Child("Amount").SetValueAsync(amount);
        }));     
    }
    public void Minus()
    {
        StartCoroutine(GetAmount((int amount) =>
        {
            amount -= 1;
            if (amount <= 0)
            {
                amount = 0;
            }
            dbReference.Child("Users").Child(user.UserId).Child("Amount").SetValueAsync(amount);
        })); 
    }
}
