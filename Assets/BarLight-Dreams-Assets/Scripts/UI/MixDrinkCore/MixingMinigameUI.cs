using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum MixingPhase
{
    AddIngredients,
    Shake,
    Complete
}

public class MixingMinigameUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject root;
    [SerializeField] private CanvasGroup rootCanvasGroup;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float rootStartScale = 0.9f;

    private Vector3 rootOriginalScale;

    [Header("Ingredient Progress")]
    [SerializeField] private GameObject ingredientRoot;
    [SerializeField] private TMP_Text ingredientProgressText;
    [SerializeField] private TMP_Text ingredientInstructionText;
    private Coroutine ingredientInstructionCoroutine;

    [Header("Shaker")]
    [SerializeField] private GameObject shakerRoot;
    [SerializeField] private Transform shakerTarget;

    [Header("Shaker Movement")]
    [SerializeField] private float shakerMoveMultiplier = 1f;
    [SerializeField] private float maxShakerOffset = 225f;
    [SerializeField] private float maxShakerRotation = -45f;

    private Vector3 shakerStartPosition;
    private Quaternion shakerStartRotation;

    [Header("Shaker UI")]
    [SerializeField] private Slider shakeProgressSlider;
    [SerializeField] private TMP_Text shakePercentText;
    [SerializeField] private TMP_Text shakeInstructionText;

    private float requiredShakeDistance;

    [Header("Complete")]
    [SerializeField] private float completeDelay = 1f;

    private DrinkRecipeSO currentRecipe;

    private int currentIndex;

    private Action onSuccess;

    private bool playing;

    private bool processingIngredient;

    private MixingPhase currentPhase;

    private float shakeDistance;
    private float leftShakeDistance;
    private float rightShakeDistance;
    private Vector3 lastMousePosition;
    private bool hasLastMousePosition;

    private void Awake()
    {
        if (root != null)
        {
            rootOriginalScale = root.transform.localScale;

            if (rootCanvasGroup == null)
            {
                rootCanvasGroup = root.GetComponent<CanvasGroup>();
            }
        }

        shakerStartPosition = shakerRoot.transform.localPosition;
        shakerStartRotation = shakerRoot.transform.localRotation;
    }

    public void StartGame(DrinkRecipeSO recipe, MixingSettings settings, Action successCallback)
    {
        currentRecipe = recipe;
        onSuccess = successCallback;

        requiredShakeDistance = settings.requiredShakeDistance;

        root.SetActive(true);
        PlayRootFadeIn();

        if (shakerRoot != null)
        {
            shakerRoot.SetActive(true);
        }

        if (ingredientRoot != null)
        {
            ingredientRoot.SetActive(true);
        }

        currentIndex = 0;

        playing = true;
        processingIngredient = false;

        currentPhase = MixingPhase.AddIngredients;

        ResetShaker();

        ResetShakeUI();

        UpdateIngredientProgress();

        CounterBarUI.instance.SetIngredientClickHandler(HandleIngredientClick);

        UIManager.Instance.LockGameplayInput();
        UIManager.Instance.LockPauseInput();

        PlayerController.instance.movement.SetCanMove(false);
    }

    private void ResetShaker()
    {
        if (shakerRoot == null)
        {
            return;
        }

        shakerRoot.transform.localPosition = shakerStartPosition;
        shakerRoot.transform.localRotation = shakerStartRotation;
    }

    private void Update()
    {
        if (!playing)
        {
            return;
        }

        if (currentPhase == MixingPhase.Shake)
        {
            UpdateShake();
        }
    }

    private bool HandleIngredientClick(GameObject clickedObject)
    {
        if (!playing)
        {
            return false;
        }

        if (currentPhase != MixingPhase.AddIngredients)
        {
            return false;
        }

        if (processingIngredient)
        {
            return false;
        }

        if (currentRecipe == null)
        {
            return false;
        }

        if (currentRecipe.ingredients == null || currentRecipe.ingredients.Count == 0)
        {
            return false;
        }

        if (currentIndex >= currentRecipe.ingredients.Count)
        {
            return false;
        }

        CounterIngredientUI ingredientUI = clickedObject.GetComponent<CounterIngredientUI>();

        if (ingredientUI == null)
        {
            return false;
        }

        IngredientType clickedIngredient = ingredientUI.IngredientType;

        IngredientType requiredIngredient = currentRecipe.ingredients[currentIndex].ingredientType;

        if (clickedIngredient != requiredIngredient)
        {
            WrongIngredientFeedback(clickedIngredient, requiredIngredient);
            return false;
        }

        CorrectIngredientFeedback(clickedIngredient);

        processingIngredient = true;

        ingredientUI.MoveToTarget(
            shakerTarget,
            () =>
            {
                CounterBarUI.instance.RemoveIngredient(clickedObject);

                currentIndex++;

                processingIngredient = false;

                UpdateIngredientProgress();

                if (currentIndex >= currentRecipe.ingredients.Count)
                {
                    AddIngredientsInShakeComplete();
                }
            }
        );

        return true;
    }

    private void CorrectIngredientFeedback(IngredientType ingredient)
    {
        if (ingredientInstructionText == null)
        {
            return;
        }

        ingredientInstructionText.text = $"Correct ingredient: {FormatIngredientName(ingredient)}";

        ShowIngredientInstruction();
    }

    private void WrongIngredientFeedback(IngredientType clicked, IngredientType required)
    {
        if (ingredientInstructionText == null)
        {
            return;
        }

        ingredientInstructionText.text = $"Wrong! Add {FormatIngredientName(required)}";

        ShowIngredientInstruction();
    }

    private void ShowIngredientInstruction()
    {
        if (ingredientInstructionCoroutine != null)
        {
            StopCoroutine(ingredientInstructionCoroutine);
        }

        ingredientInstructionText.gameObject.SetActive(true);

        ingredientInstructionCoroutine = StartCoroutine(HideIngredientInstructionAfterDelay());
    }

    private IEnumerator HideIngredientInstructionAfterDelay()
    {
        yield return new WaitForSeconds(1f);

        if (ingredientInstructionText != null)
        {
            ingredientInstructionText.gameObject.SetActive(false);
        }

        ingredientInstructionCoroutine = null;
    }

    private void UpdateIngredientProgress()
    {
        if (currentRecipe == null || currentRecipe.ingredients == null)
        {
            return;
        }

        int total = currentRecipe.ingredients.Count;

        if (ingredientProgressText != null)
        {
            ingredientProgressText.text = $"{Mathf.Min(currentIndex, total)} / {total}";
        }
    }

    private string FormatIngredientName(IngredientType ingredient)
    {
        return ingredient.ToString().Replace("_", " ");
    }

    private void AddIngredientsInShakeComplete()
    {
        CounterBarUI.instance.ClearIngredientClickHandler();

        StartShakePhase();
    }

    private void StartShakePhase()
    {
        currentPhase = MixingPhase.Shake;

        if (ingredientRoot != null)
        {
            ingredientRoot.SetActive(false);
        }

        shakeDistance = 0f;

        hasLastMousePosition = false;

        ResetShakeUI();

        if (shakeProgressSlider != null)
        {
            shakeProgressSlider.gameObject.SetActive(true);
        }

        if (shakeInstructionText != null)
        {
            shakeInstructionText.text = "Hold LMB + Move Left & Right";
        }
    }

    private void UpdateShake()
    {
        if (currentPhase != MixingPhase.Shake)
        {
            return;
        }

        if (!Input.GetMouseButton(0))
        {
            hasLastMousePosition = false;
            return;
        }

        Vector3 currentMousePosition = Input.mousePosition;

        if (!hasLastMousePosition)
        {
            lastMousePosition = currentMousePosition;
            hasLastMousePosition = true;
            return;
        }

        Vector3 mouseDelta = currentMousePosition - lastMousePosition;

        lastMousePosition = currentMousePosition;

        float actualMovementX = UpdateShakerMovement(mouseDelta);

        if (actualMovementX < 0f)
        {
            leftShakeDistance += Mathf.Abs(actualMovementX);
        }
        else if (actualMovementX > 0f)
        {
            rightShakeDistance += actualMovementX;
        }

        shakeDistance = Mathf.Min(leftShakeDistance, rightShakeDistance);

        float progress = Mathf.Clamp01(shakeDistance / requiredShakeDistance);

        UpdateShakeUI(progress);

        if (progress >= 1f)
        {
            ShakeComplete();
        }
    }

    private float UpdateShakerMovement(Vector3 mouseDelta)
    {
        if (shakerRoot == null)
        {
            return 0f;
        }

        Vector3 currentPosition = shakerRoot.transform.localPosition;

        float newX = currentPosition.x + mouseDelta.x * shakerMoveMultiplier;

        float minX = shakerStartPosition.x - maxShakerOffset;
        float maxX = shakerStartPosition.x + maxShakerOffset;

        newX = Mathf.Clamp(newX, minX, maxX);

        float actualMovementX = newX - currentPosition.x;

        shakerRoot.transform.localPosition = new Vector3(newX, shakerStartPosition.y, shakerStartPosition.z);

        float offset = newX - shakerStartPosition.x;

        float normalizedOffset = Mathf.Clamp(offset / maxShakerOffset, -1f, 1f);

        float rotationZ = normalizedOffset * maxShakerRotation;

        shakerRoot.transform.localRotation = shakerStartRotation * Quaternion.Euler(0f, 0f, rotationZ);

        return actualMovementX;
    }

    private void UpdateShakeUI(float progress)
    {
        if (shakeProgressSlider != null)
        {
            shakeProgressSlider.value = progress;
        }

        if (shakePercentText != null)
        {
            if (progress >= 1f)
            {
                shakePercentText.text = "COMPLETED!";
            }
            else
            {
                int percent = Mathf.RoundToInt(progress * 100f);

                shakePercentText.text = $"{percent}%";
            }
        }
    }

    private void ResetShakeUI()
    {
        shakeDistance = 0f;
        leftShakeDistance = 0f;
        rightShakeDistance = 0f;

        hasLastMousePosition = false;

        if (shakeProgressSlider != null)
        {
            shakeProgressSlider.minValue = 0f;
            shakeProgressSlider.maxValue = 1f;
            shakeProgressSlider.value = 0f;

            shakeProgressSlider.gameObject.SetActive(false);
        }

        if (shakePercentText != null)
        {
            shakePercentText.text = "0%";
        }

        if (shakeInstructionText != null)
        {
            shakeInstructionText.text = "Add the correct ingredient";
        }
    }

    private void ShakeComplete()
    {
        currentPhase = MixingPhase.Complete;

        hasLastMousePosition = false;

        UpdateShakeUI(1f);

        if (shakeInstructionText != null)
        {
            shakeInstructionText.text = "SHAKE COMPLETE";
        }

        StartCoroutine(CompleteMixing());
    }

    private IEnumerator CompleteMixing()
    {
        playing = false;

        ResetShaker();

        yield return new WaitForSeconds(completeDelay);

        CounterBarUI.instance.ClearIngredientClickHandler();

        yield return PlayRootFadeOut();

        root.SetActive(false);

        UIManager.Instance.UnlockGameplayInput();

        UIManager.Instance.UnlockPauseInput();

        PlayerController.instance.movement.SetCanMove(true);

        onSuccess?.Invoke();

        onSuccess = null;

        currentRecipe = null;
    }

    private void PlayRootFadeIn()
    {
        if (rootCanvasGroup == null || root == null)
        {
            return;
        }

        Transform rootTransform = root.transform;

        rootCanvasGroup.DOKill();
        rootTransform.DOKill();

        rootCanvasGroup.alpha = 0f;
        rootTransform.localScale = rootOriginalScale * rootStartScale;

        rootCanvasGroup
            .DOFade(1f, fadeInDuration)
            .SetEase(Ease.OutQuad);

        rootTransform
            .DOScale(rootOriginalScale, fadeInDuration)
            .SetEase(Ease.OutBack);
    }

    private IEnumerator PlayRootFadeOut()
    {
        if (rootCanvasGroup == null || root == null)
        {
            yield break;
        }

        Transform rootTransform = root.transform;

        rootCanvasGroup.DOKill();
        rootTransform.DOKill();

        Tween fadeTween = rootCanvasGroup
            .DOFade(0f, fadeOutDuration)
            .SetEase(Ease.InQuad);

        rootTransform
            .DOScale(rootOriginalScale * rootStartScale, fadeOutDuration)
            .SetEase(Ease.InQuad);

        yield return fadeTween.WaitForCompletion();

        rootCanvasGroup.alpha = 0f;
        rootTransform.localScale = rootOriginalScale;
    }
}