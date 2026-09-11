using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class LoadingSceneManager : MonoBehaviour
{
    public static LoadingSceneManager instance { get; private set; }

    [Header("Scene To Load")]
    [SerializeField] private string sceneToLoad = "MainMenu";

    [Header("UI")]
    [SerializeField] private Slider progressBar;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private CanvasGroup loadingCanvasGroup;

    [Header("Loading Settings")]
    [SerializeField] private float minLoadTime = 1.5f;
    [SerializeField] private float progressDuration = 0.25f;
    [SerializeField] private float fadeDuration = 0.5f;

    private AsyncOperation loadingOperation;
    private float displayedProgress;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        InitializeUI();
        StartCoroutine(LoadSceneRoutine());
    }

    private void InitializeUI()
    {
        displayedProgress = 0f;

        if (loadingCanvasGroup != null)
        {
            loadingCanvasGroup.alpha = 1f;
        }

        if (progressBar != null)
        {
            progressBar.value = 0f;
        }

        if (progressText != null)
        {
            progressText.text = "0%";
        }
    }

    private IEnumerator LoadSceneRoutine()
    {
        yield return null;

        float startTime = Time.time;

        loadingOperation = SceneManager.LoadSceneAsync(sceneToLoad);
        loadingOperation.allowSceneActivation = false;

        while (loadingOperation.progress < 0.9f)
        {
            float targetProgress = loadingOperation.progress / 0.9f;

            yield return StartCoroutine(AnimateProgress(targetProgress));
        }

        float elapsed = Time.time - startTime;

        if (elapsed < minLoadTime)
        {
            yield return new WaitForSeconds(minLoadTime - elapsed);
        }

        yield return StartCoroutine(
            AnimateProgress(1f)
        );

        if (loadingCanvasGroup != null)
        {
            yield return loadingCanvasGroup
                .DOFade(0f, fadeDuration)
                .SetEase(Ease.InOutQuad)
                .WaitForCompletion();
        }

        loadingOperation.allowSceneActivation = true;
    }

    private IEnumerator AnimateProgress(float targetProgress)
    {
        targetProgress = Mathf.Clamp01(targetProgress);

        if (targetProgress <= displayedProgress)
        {
            yield break;
        }

        float startProgress = displayedProgress;
        float elapsed = 0f;

        while (elapsed < progressDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / progressDuration);

            float currentProgress = Mathf.Lerp(startProgress, targetProgress, EaseOutCubic(t));

            displayedProgress = currentProgress;

            UpdateProgressUI(displayedProgress);

            yield return null;
        }

        displayedProgress = targetProgress;
        UpdateProgressUI(displayedProgress);
    }

    private float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }

    private void UpdateProgressUI(float progress)
    {
        progress = Mathf.Clamp01(progress);

        if (progressBar != null)
        {
            progressBar.value = progress;
        }

        if (progressText != null)
        {
            int percentage = Mathf.RoundToInt(progress * 100f);
            progressText.text = $"{percentage}%";
        }
    }

    private void OnDestroy()
    {
        if (loadingCanvasGroup != null)
        {
            loadingCanvasGroup.DOKill();
        }
    }
}