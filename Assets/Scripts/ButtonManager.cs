using System.Collections;
using UnityEngine;
using DG.Tweening;

public class ButtonManager : MonoBehaviour
{
    public CanvasGroup feedButtonCanvasGroup;
    public CanvasGroup loginButtonCanvasGroup;
    private CanvasGroup canvasGroup;
    private float fadeTime = 1f;

    private void PanelFadeIn(CanvasGroup canvas)
    {
        canvasGroup = canvas;
        canvasGroup.DOFade(1, fadeTime);
        StartCoroutine("PressButtonAnimation");
    }

    private IEnumerator PressButtonAnimation()
    {
        canvasGroup.transform.localScale = Vector3.zero;
        canvasGroup.transform.DOScale(1f, fadeTime).SetEase(Ease.InBounce);
        yield return new WaitForSeconds(0.6f);
        canvasGroup.transform.DOScale(1f, fadeTime).SetEase(Ease.OutBounce);
    }
    public void FeedButton()
    {
        PanelFadeIn(feedButtonCanvasGroup);
    }
    public void LoginButton()
    {
        PanelFadeIn(loginButtonCanvasGroup);
    }
}
