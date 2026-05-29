using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public PlayerMovement movement;
    public PlayerHealth health;

    private void Awake()
    {
        instance = this;

        movement = GetComponent<PlayerMovement>();
        health = GetComponent<PlayerHealth>();
    }
}
