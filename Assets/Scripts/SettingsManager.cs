using System.Collections;
using UnityEngine;
using DG.Tweening;

public class SettingsManager : MonoBehaviour
{
    [Header("SettingsTab")]
    public CanvasGroup canvasGroup;
    public RectTransform rectTransform;
    private float fadeTime = 0.8f;

    private void Awake()
    {
        canvasGroup.gameObject.SetActive(false);
    }

    public void OpenTab()
    {
        canvasGroup.gameObject.SetActive(true);
        canvasGroup.alpha = 0;
        
        rectTransform.transform.localPosition = new Vector3(-1000f, 0f, 0f);
        rectTransform.DOAnchorPos(new Vector2(0f, 0f), fadeTime, false).SetEase(Ease.InQuint);
        canvasGroup.DOFade(1f, fadeTime); 
        canvasGroup.blocksRaycasts = true;
    }
    public void CloseTab()
    {
        canvasGroup.alpha = 1f;
        rectTransform.transform.localPosition = new Vector3(0f, 0f, 0f);
        rectTransform.DOAnchorPos(new Vector2(-1000f, 0f), fadeTime, false).SetEase(Ease.InOutQuint);
        canvasGroup.DOFade(0, fadeTime);
        canvasGroup.blocksRaycasts = false;
        StartCoroutine("Close");
    }
    private IEnumerator Close()
    {
        yield return new WaitForSeconds(0.5f);
        canvasGroup.gameObject.SetActive(false);
    }
}
