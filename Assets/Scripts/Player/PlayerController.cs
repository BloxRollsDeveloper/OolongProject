using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(InputManager2))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Gravity Tweaks")]
    public float gravCutoff = -0.1f;
    public float gravStrength = 3f;
    public float terminalVelocity = -20f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public Vector2 groundBoxSize = new Vector2(0.8f, 0.2f);
    public LayerMask whatIsGround;

    [Header("Shooting")]
    public GameObject bullet;
    public Transform bulletSpawn;
    public float shootCooldown = 0.25f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sprite;
    private InputManager2 input;
    private bool playerIsGrounded;
    private float cooldownTimer;

    private void Awake()
    {
        rb       = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite   = GetComponent<SpriteRenderer>();
        input    = GetComponent<InputManager2>();
    }

    private void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        playerIsGrounded = Physics2D.OverlapBox(
            groundCheck.position, groundBoxSize, 0f, whatIsGround);

        HandleMovement();
        HandleJump();
        HandleShoot();
        UpdateAnimator();
    }

    private void HandleMovement()
    {
        float horizontal = input.Horizontal;
        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);
        if (horizontal != 0) sprite.flipX = horizontal < 0;
    }
    
    private void HandleJump()
    {
        if (input.Jump && playerIsGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetTrigger("jump");
        }

        if (rb.linearVelocity.y < gravCutoff) rb.gravityScale = gravStrength;
        else rb.gravityScale = 1f;

        if (rb.linearVelocity.y < terminalVelocity)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, terminalVelocity);
    }
    

    private void HandleShoot()
    {
        if ((input.Attack || input.AttackHeld) && cooldownTimer <= 0f)
        {
            cooldownTimer = shootCooldown;
            Instantiate(bullet, bulletSpawn.position, Quaternion.identity);
        }
    }

    private void UpdateAnimator()
    {
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        animator.SetFloat("speed", Mathf.Abs(rb.linearVelocity.x));
        // removed grounded bool line
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, groundBoxSize);
    }
}
