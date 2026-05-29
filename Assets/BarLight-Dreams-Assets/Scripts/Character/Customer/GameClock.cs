using TMPro;
using UnityEngine;

public class GameClock : MonoBehaviour
{
    public static GameClock Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI clockText;
    [SerializeField] private TextMeshProUGUI dayText;

    [Header("Time Settings")]
    [Tooltip("How many real seconds = 1 in-game minute")]
    [SerializeField] private float realSecondsPerGameMinute = 1f;

    [Header("Start Time")]
    [SerializeField] private int startHour = 18; // 6 PM
    [SerializeField] private int startMinute = 0;

    [Header("Closing Time")]
    [SerializeField] private int closingHour = 0; // 12 AM
    [SerializeField] private int closingMinute = 0;

    private float timer;

    // Expose current time
    public int CurrentHour { get; private set; }
    public int CurrentMinute { get; private set; }

    // Current Day
    public int CurrentDay { get; private set; } = 1;

    private bool dayEnded;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        StartNewDay();
    }

    private void Update()
    {
        if (dayEnded)
            return;

        timer += Time.deltaTime;

        if (timer >= realSecondsPerGameMinute)
        {
            timer -= realSecondsPerGameMinute;

            AddMinute();
        }
    }

    /// <summary>
    /// Add 1 in-game minute
    /// </summary>
    private void AddMinute()
    {
        CurrentMinute++;

        // Increase hour every 60 minutes
        if (CurrentMinute >= 60)
        {
            CurrentMinute = 0;
            CurrentHour++;

            // Wrap after 24
            if (CurrentHour >= 24)
            {
                CurrentHour = 0;
            }
        }

        UpdateClockUI();

        // Check closing time
        if (CurrentHour == closingHour &&
            CurrentMinute == closingMinute)
        {
            EndDay();
        }
    }

    /// <summary>
    /// Update TMP clock text
    /// </summary>
    private void UpdateClockUI()
    {
        int displayHour = CurrentHour;
        string period = "AM";

        // Convert to 12-hour format
        if (displayHour >= 12)
        {
            period = "PM";
        }

        if (displayHour == 0)
        {
            displayHour = 12;
        }
        else if (displayHour > 12)
        {
            displayHour -= 12;
        }

        string formattedTime = $"{displayHour:00}:{CurrentMinute:00} {period}";

        clockText.text = formattedTime;

        dayText.text = $"Day {CurrentDay}";
    }

    /// <summary>
    /// Called when the bar closes
    /// </summary>
    private void EndDay()
    {
        dayEnded = true;

        Debug.Log("Bar Closed - End Day");

        // TODO:
        // - Stop customer spawning
        // - Make customers leave
        // - Show summary UI
        // - Save earnings

        // TEST:
        Invoke(nameof(StartNextDay), 3f);
    }

    private void StartNextDay()
    {
        CurrentDay++;

        StartNewDay();
    }

    private void StartNewDay()
    {
        dayEnded = false;

        CurrentHour = startHour;
        CurrentMinute = startMinute;

        timer = 0f;

        UpdateClockUI();

        Debug.Log($"Start Day {CurrentDay}");
    }
}