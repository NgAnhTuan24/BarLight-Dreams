using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHP = 3;
    private int currentHP;

    private HeartUI heartUI;

    public int CurrentHP => currentHP;

    private void Awake()
    {
        heartUI = FindFirstObjectByType<HeartUI>();

        currentHP = maxHP;
    }

    private void Start()
    {
        heartUI.Initialize(maxHP);
        heartUI.UpdateHearts(currentHP);

    }

    public void SetHP(int hp)
    {
        currentHP = Mathf.Clamp(hp, 0, maxHP);
        heartUI.UpdateHearts(currentHP);
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        heartUI.UpdateHearts(currentHP);

        if (currentHP <= 0)
        {
            FailDay();
        }
    }

    void FailDay()
    {
        PlayerController.instance.movement.SetCanMove(false);
        CustomerManager.instance.ForceAllCustomersLeave();
        FailDayUIHandler.instance.ShowFailDay();
    }
}
