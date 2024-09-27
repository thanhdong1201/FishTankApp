using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class NoficationManager : MonoBehaviour
{
    [Header("Alert")]
    public CanvasGroup canvasGroup;
    public RectTransform rectTransform;
    [Header("Text")]
    public TextMeshProUGUI noficationText;
    private string manualMode = "Auto mode is Off! You have to feed by hand!";
    private string autoMode = "Auto mode is On! You dont need to feed!";
    private string success = "Your fish has been fed!";
    private string alert = "Your fish has been fed recently! You cannot continue feeding!";
    private string resetProgress = "Your progress has been reset!";
    private float fadeTime = 0.8f;

    private SoundManager soundManager;

    private void Awake()
    {
        soundManager = GetComponent<SoundManager>();
        canvasGroup.gameObject.SetActive(false);
    }
    public void ManualMode()
    {
        noficationText.text = manualMode;
        PanelFadeIn();
        soundManager.PlayNoficationSound();
    }
    public void AutoMode()
    {
        noficationText.text = autoMode;
        PanelFadeIn();
        soundManager.PlayNoficationSound();
    }
    public void Alert()
    {
        noficationText.text = alert;
        PanelFadeIn();
        soundManager.PlayNoficationSound();
    }
    public void Success()
    {
        noficationText.text = success;
        PanelFadeIn();
        soundManager.PlayLoginSound();
    }
    public void ResetProgress()
    {
        noficationText.text = resetProgress;
        PanelFadeIn();
        soundManager.PlayNoficationSound();
    }
    public void Close()
    {
        PanelFadeOut();
    }
    private void PanelFadeIn()
    {
        canvasGroup.gameObject.SetActive(true);
        canvasGroup.alpha = 0;
        rectTransform.transform.localPosition = new Vector3(0f, -1000f, 0f);
        rectTransform.DOAnchorPos(new Vector2(0f, 0f), fadeTime, false).SetEase(Ease.OutElastic);
        canvasGroup.DOFade(1, fadeTime);
        StartCoroutine("FeedAnimation");
        canvasGroup.blocksRaycasts = true;
    }
    private void PanelFadeOut()
    {
        canvasGroup.alpha = 1f;
        rectTransform.transform.localPosition = new Vector3(0f, 0f, 0f);
        rectTransform.DOAnchorPos(new Vector2(0f, -1000f), fadeTime, false).SetEase(Ease.InOutQuint);
        canvasGroup.DOFade(0, fadeTime);
        canvasGroup.blocksRaycasts = false;
    }
    private IEnumerator FeedAnimation()
    {
        canvasGroup.transform.localScale = Vector3.zero;
        canvasGroup.transform.DOScale(1.2f, fadeTime).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(0.3f);
    }
}
