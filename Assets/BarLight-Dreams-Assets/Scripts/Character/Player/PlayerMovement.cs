using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    [Header("Footstep Sounds")]
    [SerializeField] private AudioClip footstepClip;
    [SerializeField] private float stepInterval = 0.4f;

    Rigidbody2D rb2D;
    Animator animator;

    private Vector2 moveDirection;
    private Vector2 animDirection = new Vector2(0, -1);

    private bool canMove = true;

    private float stepTimer;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (canMove)
        {
            moveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            
            if (moveDirection != Vector2.zero)
            {
                animDirection = moveDirection;

                HandleFootstepSound();
            }
            else
            {
                stepTimer = 0f;
            }
        }
        else
        {
            moveDirection = Vector2.zero;
            stepTimer = 0f;
        }

        UpdateAnim();
    }

    private void FixedUpdate()
    {
        rb2D.velocity = moveDirection * moveSpeed;
    }

    private void UpdateAnim()
    {
        animator.SetFloat("MoveX", animDirection.x);
        animator.SetFloat("MoveY", animDirection.y);
        animator.SetFloat("Speed", moveDirection.sqrMagnitude);
    }

    private void HandleFootstepSound()
    {
        if (moveDirection.sqrMagnitude < 0.1f) return;
        if (footstepClip == null) return;

        stepTimer -= Time.deltaTime;

        if (stepTimer <= 0f)
        {
            AudioManager.instance.PlaySFX(footstepClip);
            stepTimer = stepInterval;
        }
    }

    public void SetCanMove(bool value)
    {
        canMove = value;

        if (!canMove)
        {
            moveDirection = Vector2.zero;
            rb2D.velocity = Vector2.zero;
            stepTimer = 0f;
        }
    }
}