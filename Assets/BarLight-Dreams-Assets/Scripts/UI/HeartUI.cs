using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HeartUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image heartPrefab;
    [SerializeField] private Transform container;

    [Header("Animation")]
    [SerializeField] private float flyDistance = 100f;
    [SerializeField] private float duration = 0.5f;

    private List<Image> hearts = new();

    public void Initialize(int maxHealth)
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        hearts.Clear();

        for (int i = 0; i < maxHealth; i++)
        {
            Image newHeart = Instantiate(heartPrefab, container);

            hearts.Add(newHeart);
        }
    }

    public void UpdateHearts(int currentHealth)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            bool shouldShow = i < currentHealth;

            if (!shouldShow && hearts[i].enabled)
            {
                PlayLoseHeartAnimation(hearts[i]);
            }
            else if (shouldShow)
            {
                hearts[i].enabled = true;
            }
        }
    }

    private void PlayLoseHeartAnimation(Image heart)
    {
        RectTransform rect = heart.rectTransform;

        Vector2 originalPos = rect.anchoredPosition;
        Vector3 originalScale = rect.localScale;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(rect.DOScale(1.2f, 0.1f));

        sequence.Append(
            rect.DOAnchorPos(
                originalPos + new Vector2(Random.Range(-50f, 50f), flyDistance),
                duration
            ).SetEase(Ease.OutQuad)
        );

        sequence.Join(rect.DORotate(new Vector3(0, 0, Random.Range(-45f, 45f)), duration));

        sequence.Join(heart.DOFade(0f, duration));

        sequence.OnComplete(() =>
        {
            heart.enabled = false;

            rect.anchoredPosition = originalPos;
            rect.localScale = originalScale;
            rect.rotation = Quaternion.identity;

            Color color = heart.color;
            color.a = 1f;
            heart.color = color;
        });
    }
}