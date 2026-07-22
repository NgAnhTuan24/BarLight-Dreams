using System.Collections;
using UnityEngine;

public class CustomerPatience : MonoBehaviour
{
    [Header("Bubble")]
    [SerializeField] private GameObject timerBubble;
    [SerializeField] private SpriteRenderer timerIcon;

    [Header("Timer Frames")]
    [SerializeField] private Sprite[] timerFrames;

    private CustomerController customer;

    private Coroutine patienceRoutine;

    public float PatiencePercentUsed { get; private set; }

    private void Awake()
    {
        customer = GetComponent<CustomerController>();

        timerBubble.SetActive(false);
    }

    public void StartWaitingOrder()
    {
        StartPatience(customer.Data.waitOrderTime);
    }

    public void StartWaitingDrink()
    {
        StartPatience(customer.Data.waitDrinkTime);
    }

    public void StopPatience()
    {
        if (patienceRoutine != null)
        {
            StopCoroutine(patienceRoutine);
        }

        timerBubble.SetActive(false);
    }

    void StartPatience(float duration)
    {
        if (patienceRoutine != null)
        {
            StopCoroutine(patienceRoutine);
        }

        patienceRoutine = StartCoroutine(PatienceRoutine(duration));
    }

    IEnumerator PatienceRoutine(float duration)
    {
        timerBubble.SetActive(true);

        float timer = duration;

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            float percent = 1f - (timer / duration);

            PatiencePercentUsed = percent;

            UpdateTimerVisual(percent);

            yield return null;
        }

        timerBubble.SetActive(false);

        customer.LeaveAngry();
    }

    void UpdateTimerVisual(float percent)
    {
        if (timerFrames.Length == 0)
            return;

        int index = Mathf.Clamp(Mathf.FloorToInt(percent * timerFrames.Length), 0, timerFrames.Length - 1);

        timerIcon.sprite = timerFrames[index];
    }
}