using System.Collections;
using TMPro;
using UnityEngine;

public class CustomerTypePopupText : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text popupText;

    [Header("Display")]
    [SerializeField] private float fadeInDuration = 0.25f;
    [SerializeField] private float showDuration = 1.5f;
    [SerializeField] private float fadeOutDuration = 0.25f;

    private Coroutine popupRoutine;

    private void Awake()
    {
        canvasGroup.alpha = 0f;
    }

    public void Show(CustomerType customerType)
    {
        popupText.color = GetTypeColor(customerType);

        Show(customerType.ToString());
    }

    public void Show(string text)
    {
        if (popupRoutine != null)
        {
            StopCoroutine(popupRoutine);
        }

        popupRoutine = StartCoroutine(ShowRoutine(text));
    }

    private IEnumerator ShowRoutine(string text)
    {
        popupText.text = text;

        canvasGroup.alpha = 0f;

        float timer = 0f;

        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / fadeInDuration);

            canvasGroup.alpha = t;

            yield return null;
        }

        canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(showDuration);

        timer = 0f;

        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / fadeOutDuration);

            canvasGroup.alpha = 1f - t;

            yield return null;
        }

        canvasGroup.alpha = 0f;

        popupRoutine = null;
    }

    private Color GetTypeColor(CustomerType customerType)
    {
        switch (customerType)
        {
            case CustomerType.Normal:
                return Color.white;

            case CustomerType.Student:
                return new Color(0.3f, 0.6f, 1f);

            case CustomerType.Impatient:
                return new Color(1f, 0.3f, 0.3f);

            case CustomerType.Tourist:
                return new Color(1f, 0.65f, 0.2f);

            case CustomerType.Rich:
                return new Color(0.3f, 0.85f, 0.4f);

            case CustomerType.VIP:
                return new Color(1f, 0.8f, 0.2f);

            default:
                return Color.white;
        }
    }
}