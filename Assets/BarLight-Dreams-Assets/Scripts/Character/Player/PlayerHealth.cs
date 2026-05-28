using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHP = 3;
    private int currentHP;

    private HeartUI heartUI;

    private void Awake()
    {
        heartUI = FindFirstObjectByType<HeartUI>();
    }

    private void Start()
    {
        currentHP = maxHP;

        heartUI.Initialize(maxHP);
        heartUI.UpdateHearts(currentHP);

    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        heartUI.UpdateHearts(currentHP);

        if (currentHP < 0)
        {
        }
    }
}
