using System.Collections;
using TMPro;
using UnityEngine;

public class CustomerPopupText : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text popupText;

    [Header("Animation")]
    [SerializeField] private float moveUpDistance = 1f;
    [SerializeField] private float duration = 4f;

    private Coroutine popupRoutine;

    private Vector3 startPos;

    private void Awake()
    {
        startPos = transform.localPosition;

        canvasGroup.alpha = 0f;
    }

    public void ShowText(string text)
    {
        if (popupRoutine != null)
        {
            StopCoroutine(popupRoutine);
        }

        popupRoutine = StartCoroutine(ShowRoutine(text));
    }

    IEnumerator ShowRoutine(string text)
    {
        popupText.text = text;

        float timer = 0f;

        transform.localPosition = startPos;

        canvasGroup.alpha = 1f;

        Vector3 targetPos = startPos + Vector3.up * moveUpDistance;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

            transform.localPosition = Vector3.Lerp(startPos, targetPos, t);

            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        canvasGroup.alpha = 0f;

        transform.localPosition = startPos;
    }
}