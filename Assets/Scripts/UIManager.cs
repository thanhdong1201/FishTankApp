using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public GameObject loadingUI;
    public GameObject mainScreen;

    [Header("SetTimeScreen")]
    public CanvasGroup setFeedTimeScreen;
    public CanvasGroup setLightTimeScreen;

    [Header("Login")]
    public CanvasGroup loginUICanvasGroup;
    public RectTransform loginUIRectTransform;

    [Header("SettingsTab")]
    public CanvasGroup settingTabCanvasGroup;
    public RectTransform settingTabRectTransform;
    private float fadeTime = 0.5f;

    private void Awake()
    {
        loginUICanvasGroup.gameObject.SetActive(true);

        settingTabCanvasGroup.gameObject.SetActive(true);
        setFeedTimeScreen.gameObject.SetActive(true);
        setLightTimeScreen.gameObject.SetActive(true);

        settingTabCanvasGroup.alpha = 0f;
        setFeedTimeScreen.alpha = 0f;
        setLightTimeScreen.alpha = 0f;
    }

    public void ClearScreen()
    {
        mainScreen.SetActive(false);
        loadingUI.SetActive(false);
        settingTabCanvasGroup.gameObject.SetActive(false);
    }
    public void LoginScreen()
    {
        //ClearScreen();
        LogOut();
    }
    public void EnterMainScreen()
    {
        ClearScreen();
        mainScreen.SetActive(true);
        LoginSuccessful();
    }
    public void LoadingScreen(bool state)
    {
        loadingUI.SetActive(state);
    }
    private void OnApplicationQuit()
    {
        
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void LoginSuccessful()
    {
        loginUICanvasGroup.gameObject.SetActive(true);
        loginUICanvasGroup.alpha = 1f;
        loginUICanvasGroup.transform.localPosition = new Vector3(0f, 0f, 0f);
        loginUIRectTransform.DOAnchorPos(new Vector2(0f, 3000f), 1.5f, false).SetEase(Ease.OutCubic);
        loginUICanvasGroup.DOFade(0f, 2f);
        loginUICanvasGroup.blocksRaycasts = false;
        StartCoroutine("LoginSuccessfulAnimation");
    }
    public void LogOut()
    {
        loginUICanvasGroup.gameObject.SetActive(true);
        loginUICanvasGroup.alpha = 0f;
        loginUICanvasGroup.transform.localPosition = new Vector3(0f, 3000f, 0f);
        loginUIRectTransform.DOAnchorPos(new Vector2(0f, 0f), 1.5f, false).SetEase(Ease.OutCubic);
        loginUICanvasGroup.DOFade(1f, 2f);
        loginUICanvasGroup.blocksRaycasts = true;
    }

    private IEnumerator LoginSuccessfulAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        loginUICanvasGroup.gameObject.SetActive(false);
    }
    public void OpenTab()
    {
        settingTabCanvasGroup.gameObject.SetActive(true);
        settingTabCanvasGroup.alpha = 0;

        settingTabRectTransform.transform.localPosition = new Vector3(-1000f, 0f, 0f);
        settingTabRectTransform.DOAnchorPos(new Vector2(0f, 0f), fadeTime, false).SetEase(Ease.InQuint);
        settingTabCanvasGroup.DOFade(1f, fadeTime);
        settingTabCanvasGroup.blocksRaycasts = true;
    }
    public void CloseTab()
    {
        settingTabCanvasGroup.alpha = 1f;
        settingTabRectTransform.transform.localPosition = new Vector3(0f, 0f, 0f);
        settingTabRectTransform.DOAnchorPos(new Vector2(-1000f, 0f), fadeTime, false).SetEase(Ease.InOutQuint);
        settingTabCanvasGroup.DOFade(0, fadeTime);
        settingTabCanvasGroup.blocksRaycasts = false;
        StartCoroutine("Close");
    }
    private IEnumerator Close()
    {
        yield return new WaitForSeconds(0.5f);
        settingTabCanvasGroup.gameObject.SetActive(false);
    }
    //Set time screen
    public void OpenSetFeedTimeScreen()
    {
        setFeedTimeScreen.alpha = 1f;
        setFeedTimeScreen.blocksRaycasts = true;
    }
    public void CloseSetFeedTimeScreen()
    {
        setFeedTimeScreen.alpha = 0f;
        setFeedTimeScreen.blocksRaycasts = false;
    }
    public void OpenSetLightTimeScreen()
    {
        setLightTimeScreen.alpha = 1f;
        setLightTimeScreen.blocksRaycasts = true;
    }
    public void CloseSetLightTimeScreen()
    {
        setLightTimeScreen.alpha = 0f;
        setLightTimeScreen.blocksRaycasts = false;
    }
}
