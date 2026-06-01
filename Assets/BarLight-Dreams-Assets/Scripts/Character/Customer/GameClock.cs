using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameClock : MonoBehaviour
{
    public static GameClock instance { get; private set; }

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

    [Header("Test Time Speed")]
    [SerializeField] private bool isFastForward;
    [SerializeField] private float fastForwardMultiplier = 2f;

    [Header("UI In Game")]
    [SerializeField] private DayIntroUI dayIntroUI;
    [SerializeField] private SummaryDayUI summaryUI;
    public bool IsRunning { get; private set; }

    private float timer;

    // Expose current time
    public int CurrentHour { get; private set; }
    public int CurrentMinute { get; private set; }

    // Current Day
    public int CurrentDay { get; private set; } = 1;

    public bool IsRushHour
    {
        get
        {
            return CurrentHour == 22 || (CurrentHour == 23 && CurrentMinute <= 30);
        }
    }

    public bool CanReceiveCustomers
    {
        get
        {
            return !(CurrentHour == 23 && CurrentMinute >= 45);
        }
    }

    private bool dayEnded;

    public event Action OnNewDayStarted;
    public event Action OnBarClosed;

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
        StartNewDay();
    }

    private void Update()
    {
        if (PlayerController.instance.health.CurrentHP == 0) return;

        if (dayEnded || !IsRunning) return;

        float speedMultiplier = isFastForward ? fastForwardMultiplier : 1f;

        timer += Time.deltaTime * speedMultiplier;

        while (timer >= realSecondsPerGameMinute)
        {
            timer -= realSecondsPerGameMinute;

            AddMinute();
        }
    }

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

    private void EndDay()
    {
        StartCoroutine(EndDayRoutine());
    }

    private IEnumerator EndDayRoutine()
    {
        dayEnded = true;
        IsRunning = false;

        PlayerController.instance.movement.SetCanMove(false);

        OnBarClosed?.Invoke();

        yield return new WaitUntil(
            () => CustomerManager.instance.CurrentCustomerCount == 0
        );

        bool introFinished = false;

        dayIntroUI.Show(
            "END DAY",
            "CLOSE BAR",
            () => introFinished = true
        );

        yield return new WaitUntil(() => introFinished);

        summaryUI.Show(
            CurrentDay,
            0,
            0,
            StartNextDay
        );
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

        IsRunning = false;

        PlayerController.instance.movement.SetCanMove(false);

        UpdateClockUI();

        dayIntroUI.Show(
            $"DAY {CurrentDay}",
            "OPEN BAR",
            () =>
            {
                IsRunning = true;

                PlayerController.instance.movement.SetCanMove(true);

                OnNewDayStarted?.Invoke();

                Debug.Log($"Start Day {CurrentDay}");
            }
        );
    }
}