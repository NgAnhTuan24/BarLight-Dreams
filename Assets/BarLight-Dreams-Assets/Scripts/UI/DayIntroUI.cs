using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class DayIntroUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text titleText;

    [Header("Animation")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float messageDuration = 1f;

    private Sequence currentSequence;

    public void Show(string firstMessage, string secondMessage, Action onComplete = null)
    {
        gameObject.SetActive(true);

        currentSequence?.Kill();

        canvasGroup.alpha = 0f;
        titleText.alpha = 0f;

        currentSequence = DOTween.Sequence();

        currentSequence.AppendCallback(() =>
        {
            titleText.text = firstMessage;
        });

        currentSequence.Append(canvasGroup.DOFade(1f, fadeDuration));
        currentSequence.Join(titleText.DOFade(1f, fadeDuration));

        currentSequence.AppendInterval(messageDuration);

        currentSequence.Append(titleText.DOFade(0f, fadeDuration * 0.5f));

        currentSequence.AppendCallback(() =>
        {
            titleText.text = secondMessage;
        });

        currentSequence.Append(titleText.DOFade(1f, fadeDuration * 0.5f));

        currentSequence.AppendInterval(messageDuration);

        currentSequence.Append(canvasGroup.DOFade(0f, fadeDuration));
        currentSequence.Join(titleText.DOFade(0f, fadeDuration));

        currentSequence.OnComplete(() =>
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
        });
    }

    private void OnDestroy()
    {
        currentSequence?.Kill();
    }
}