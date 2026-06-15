using DG.Tweening;
using UnityEngine;

public class UIPopup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform panel;

    [Header("Hide When Open")]
    [SerializeField] private GameObject[] hideObjects;

    [Header("Animation")]
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float hiddenY = -1080f;

    [Header("Pause")]
    [SerializeField] private bool pauseGameplay = false;

    private Vector2 shownPosition;
    private Tween currentTween;

    private static UIPopup currentOpenPopup;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        shownPosition = panel.anchoredPosition;
    }

    public void Open()
    {
        if (SceneTransition.instance != null && SceneTransition.instance.IsTransitioning) return;

        if (IsOpen) return;

        if (currentOpenPopup != null && currentOpenPopup != this) return;

        currentOpenPopup = this;

        IsOpen = true;

        gameObject.SetActive(true);

        if (pauseGameplay)
        {
            Time.timeScale = 0f;
            PlayerController.instance.movement.SetCanMove(false);
        }

        foreach (GameObject obj in hideObjects)
        {
            obj.SetActive(false);
        }

        currentTween?.Kill();

        panel.anchoredPosition = new Vector2(shownPosition.x, hiddenY);

        currentTween = panel.DOAnchorPos(shownPosition, duration).SetEase(Ease.OutCubic).SetUpdate(true);
    }

    public void Close()
    {
        if (!IsOpen) return;

        IsOpen = false;

        if (currentOpenPopup == this)
        {
            currentOpenPopup = null;
        }

        if (pauseGameplay)
        {
            Time.timeScale = 1f;
            PlayerController.instance.movement.SetCanMove(true);
        }

        currentTween?.Kill();

        currentTween = panel.DOAnchorPos(new Vector2(shownPosition.x, hiddenY), duration)
            .SetEase(Ease.InCubic)
            .SetUpdate(true)
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

    //private void OnDisable()
    //{
    //    if (pauseGameplay)
    //    {
    //        Time.timeScale = 1f;
    //    }
    //}
}
