using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    private Rigidbody2D rb;
    private float horizontalInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        ReadInput();
        FlipSprite();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    private void ReadInput()
    {
        horizontalInput = 0f;

        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                horizontalInput -= 1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                horizontalInput += 1f;
        }

        Gamepad gamepad = Gamepad.current;
        if (gamepad != null && Mathf.Approximately(horizontalInput, 0f))
        {
            float stick = gamepad.leftStick.x.ReadValue();
            if (Mathf.Abs(stick) > 0.2f)
                horizontalInput = Mathf.Sign(stick);
        }
    }

    private void FlipSprite()
    {
        if (spriteRenderer == null) return;

        if (horizontalInput > 0f)
            spriteRenderer.flipX = true;
        else if (horizontalInput < 0f)
            spriteRenderer.flipX = false;
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;
        animator.SetFloat(SpeedHash, Mathf.Abs(horizontalInput));
    }
}