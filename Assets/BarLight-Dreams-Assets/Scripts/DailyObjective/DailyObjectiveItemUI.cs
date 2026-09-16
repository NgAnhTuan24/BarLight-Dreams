using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyObjectiveItemUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text objectiveTypeText;
    [SerializeField] private TMP_Text objectiveText;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TMP_Text progressPercentText;

    public void SetObjective(DailyObjective objective)
    {
        if (objective == null)
        {
            ClearUI();
            return;
        }

        if (objectiveTypeText != null)
        {
            objectiveTypeText.text = GetObjectiveTypeName(objective.type);
        }

        if (objectiveText != null)
        {
            objectiveText.text = DailyObjectiveManager.instance.GetObjectiveDescription(objective);
        }

        float progress = 0f;

        if (objective.target > 0)
        {
            progress = (float)objective.progress / objective.target;
        }

        progress = Mathf.Clamp01(progress);

        if (progressBar != null)
        {
            progressBar.value = progress;
        }

        if (progressPercentText != null)
        {
            if (objective.IsCompleted)
            {
                progressPercentText.text = "COMPLETED!";
            }
            else
            {
                progressPercentText.text = $"{Mathf.RoundToInt(progress * 100f)}%";
            }
        }

        if (objective.IsCompleted)
        {
            if (progressText != null)
            {
                progressText.gameObject.SetActive(false);
            }

            if (rewardText != null)
            {
                rewardText.gameObject.SetActive(true);

                rewardText.text = $"REWARD: +{objective.reward} Money";
            }
        }
        else
        {
            if (progressText != null)
            {
                progressText.gameObject.SetActive(true);

                progressText.text = $"Progress: " + $"{objective.progress} / " + $"{objective.target}";
            }

            if (rewardText != null)
            {
                rewardText.gameObject.SetActive(false);
                rewardText.text = "";
            }
        }
    }

    public void ClearUI()
    {
        if (objectiveTypeText != null)
        {
            objectiveTypeText.text = "";
        }

        if (objectiveText != null)
        {
            objectiveText.text = "";
        }

        if (progressText != null)
        {
            progressText.gameObject.SetActive(true);
            progressText.text = "";
        }

        if (rewardText != null)
        {
            rewardText.gameObject.SetActive(false);
            rewardText.text = "";
        }

        if (progressBar != null)
        {
            progressBar.value = 0f;
        }

        if (progressPercentText != null)
        {
            progressPercentText.text = "0%";
        }
    }

    private string GetObjectiveTypeName(DailyObjectiveType type)
    {
        switch (type)
        {
            case DailyObjectiveType.ServeCustomers:
            case DailyObjectiveType.ServeCustomerType:
                return "Serve Customers";

            case DailyObjectiveType.MixDrink:
                return "Mix Drink";

            case DailyObjectiveType.ServeDrink:
                return "Serve Drink";

            case DailyObjectiveType.EarnMoney:
                return "Earn Money";
        }

        return "Daily Objective";
    }
}