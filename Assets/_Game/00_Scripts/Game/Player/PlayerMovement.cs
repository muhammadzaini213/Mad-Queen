using UnityEngine;
using UnityEngine.InputSystem;
using Slafurry.Systems.Audio;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    [Header("Audio")]
    [SerializeField] private SFXReference footstepSFX;

    private static readonly int IsMovingHash = Animator.StringToHash("isMoving");

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isFootstepPlaying;

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
        UpdateFootstepAudio();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(
            horizontalInput * moveSpeed,
            rb.linearVelocity.y
        );
    }

    private void OnDisable()
    {
        StopFootstep();
    }

    private void ReadInput()
    {
        horizontalInput = 0f;

        if (DialogHUD.IsBusy) return;

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

        bool isMoving = !Mathf.Approximately(horizontalInput, 0f);

        animator.SetBool(IsMovingHash, isMoving);
    }

    private void UpdateFootstepAudio()
    {
        bool isMoving = !Mathf.Approximately(horizontalInput, 0f);

        if (isMoving && !isFootstepPlaying)
            StartFootstep();
        else if (!isMoving && isFootstepPlaying)
            StopFootstep();
    }

    private void StartFootstep()
    {
        if (footstepSFX == null) return;

        Audio.PlaySFX2D(footstepSFX, loop: true);
        isFootstepPlaying = true;
    }

    private void StopFootstep()
    {
        if (!isFootstepPlaying) return;

        Audio.StopSFX(footstepSFX);
        isFootstepPlaying = false;
    }
}