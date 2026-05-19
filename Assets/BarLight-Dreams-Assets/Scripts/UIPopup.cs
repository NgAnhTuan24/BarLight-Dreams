using DG.Tweening;
using UnityEngine;

public class UIPopup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform panel;

    [Header("Hide When Open")]
    [SerializeField] private GameObject[] hideObjects;

    [Header("Animation")]
    [SerializeField] private float duration = 0.4f;
    [SerializeField] private float hiddenY = -1000f;

    private Vector2 shownPosition;
    private Tween currentTween;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        shownPosition = panel.anchoredPosition;
    }

    public void Open()
    {
        if (IsOpen) return;

        IsOpen = true;

        gameObject.SetActive(true);

        foreach (GameObject obj in hideObjects)
        {
            obj.SetActive(false);
        }

        currentTween?.Kill();

        panel.anchoredPosition = new Vector2(shownPosition.x, hiddenY);

        currentTween = panel.DOAnchorPos(shownPosition, duration).SetEase(Ease.OutCubic);
    }

    public void Close()
    {
        if (!IsOpen) return;

        IsOpen = false;

        currentTween?.Kill();

        currentTween = panel.DOAnchorPos(new Vector2(shownPosition.x, hiddenY), duration)
            .SetEase(Ease.InCubic)
            .OnComplete(() =>
            {
                foreach (GameObject obj in hideObjects)
                {
                    obj.SetActive(true);
                }

                gameObject.SetActive(false);
            });
    }

    public void Toggle()
    {
        if (IsOpen)
            Close();
        else
            Open();
    }
}