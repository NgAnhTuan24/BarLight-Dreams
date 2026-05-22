using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    Rigidbody2D rb2D;
    Animator animator;

    private Vector2 moveDirection;
    private Vector2 animDirection = new Vector2(0, -1);

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        moveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        if (moveDirection != Vector2.zero)
        {
            animDirection = moveDirection;
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
}