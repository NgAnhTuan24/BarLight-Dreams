using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public PlayerMovement movement;

    private void Awake()
    {
        instance = this;

        movement = GetComponent<PlayerMovement>();
    }
}
