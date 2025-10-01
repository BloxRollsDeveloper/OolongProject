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

    [Header("Platform Drop")]
    public float dropTime = 0.3f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sprite;
    private InputManager2 input;
    private bool playerIsGrounded;
    private float cooldownTimer;

    private Collider2D playerCollider;
    private Collider2D currentPlatform;

    private void Awake()
    {
        rb       = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite   = GetComponent<SpriteRenderer>();
        input    = GetComponent<InputManager2>();
        playerCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        playerIsGrounded = Physics2D.OverlapBox(
            groundCheck.position, groundBoxSize, 0f, whatIsGround);

        HandleMovement();
        HandleJump();
        HandleShoot();
        HandleDropDown(); //
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

    private void HandleDropDown()
    {
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            Collider2D platform = Physics2D.OverlapBox(
                groundCheck.position, groundBoxSize, 0f, whatIsGround);

            if (platform != null && platform.CompareTag("Platform"))
            {
                StartCoroutine(DropDownThroughPlatform(platform));
            }
        }
    }

    private System.Collections.IEnumerator DropDownThroughPlatform(Collider2D platform)
    {
        if (playerCollider != null && platform != null)
        {
            Physics2D.IgnoreCollision(playerCollider, platform, true);
            yield return new WaitForSeconds(dropTime);
            Physics2D.IgnoreCollision(playerCollider, platform, false);
        }
    }


    private void UpdateAnimator()
    {
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platform"))
        {
            currentPlatform = collision.collider;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider == currentPlatform)
        {
            currentPlatform = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, groundBoxSize);
    }
}
