using DG.Tweening;
using System;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float delayBeforeFade = 1f;
    [SerializeField] private float fadeDuration = 1f;

    public bool IsTransitioning { get; private set; }

    public void FadeIn(Action onComplete = null)
    {
        IsTransitioning = true;

        Sequence sequence = DOTween.Sequence();

        sequence.AppendInterval(delayBeforeFade);

        sequence.Append(
            canvasGroup
                .DOFade(0f, fadeDuration)
                .SetEase(Ease.Linear)
        );

        sequence.OnComplete(() =>
        {
            IsTransitioning = false;

            gameObject.SetActive(false);

            onComplete?.Invoke();
        });
    }

    public void FadeOut(Action onComplete = null)
    {
        canvasGroup
            .DOFade(1f, fadeDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                onComplete?.Invoke();
            });
    }
}
