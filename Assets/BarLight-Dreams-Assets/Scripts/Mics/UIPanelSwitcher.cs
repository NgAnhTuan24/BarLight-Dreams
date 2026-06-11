using DG.Tweening;
using UnityEngine;

public class UIPanelSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject panelToShow;
    [SerializeField] private CanvasGroup panelCanvasGroup;

    [SerializeField] private GameObject[] objectsToHide;
    [SerializeField] private GameObject[] objectsToShowWhenClose;

    [SerializeField] private float duration = 0.25f;

    private bool isTransitioning;

    public void Open()
    {
        if (isTransitioning || panelToShow.activeSelf) return;

        isTransitioning = true;

        panelToShow.SetActive(true);

        panelCanvasGroup.alpha = 0;
        panelToShow.transform.localScale = Vector3.one * 0.8f;

        panelCanvasGroup.DOFade(1, duration);
        panelToShow.transform
            .DOScale(1f, duration)
            .SetEase(Ease.OutBack).
            OnComplete(() =>
            {
                isTransitioning = false;
            });

        foreach (var obj in objectsToHide)
        {
            HideEffect(obj);
        }
    }

    public void Close()
    {
        if (isTransitioning || !panelToShow.activeSelf) return;

        isTransitioning = true;

        panelCanvasGroup.DOFade(0, duration);

        panelToShow.transform
            .DOScale(0.8f, duration)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                panelToShow.SetActive(false);
                isTransitioning = false;
            });

        foreach (var obj in objectsToShowWhenClose)
        {
            ShowEffect(obj);
        }
    }

    private void HideEffect(GameObject obj)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();

        if (cg == null) cg = obj.AddComponent<CanvasGroup>();

        cg.DOFade(0, duration).OnComplete(() => obj.SetActive(false));

        obj.transform.DOScale(0.8f, duration);
    }

    private void ShowEffect(GameObject obj)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();

        if (cg == null) cg = obj.AddComponent<CanvasGroup>();

        obj.SetActive(true);

        cg.alpha = 0;
        obj.transform.localScale = Vector3.one * 0.8f;

        cg.DOFade(1, duration);
        obj.transform.DOScale(1f, duration).SetEase(Ease.OutBack);
    }
}