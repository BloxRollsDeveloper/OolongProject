using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Vector2 groundBoxSize = new Vector2(0.8f, 0.2f);
    public LayerMask whatIsGround;
    public Transform groundCheck;
    public bool playerIsGrounded;
    

    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private SpriteRenderer _sprite;
    private float horizontalInput;

    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
       
        horizontalInput = Input.GetAxisRaw("Horizontal");

        
        playerIsGrounded = Physics2D.OverlapBox(transform.position, groundBoxSize, 0f, whatIsGround);
        if (Input.GetButtonDown("Jump") && playerIsGrounded)
        {
            _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, jumpForce);
        }

        
        if (horizontalInput != 0)
            _sprite.flipX = horizontalInput < 0;

        
        UpdateAnimation();
        if (Physics2D.OverlapBox(groundCheck.position, groundBoxSize, 0f, whatIsGround))
        {
            playerIsGrounded = true;
        }
        else
        {
            playerIsGrounded = false;
        }
        
        if (Input.Jump && playerIsGrounded)
        {
            _rigidbody2D.linearVelocityY = jumpForce;
        }
    }

    void FixedUpdate()
    {
        // Horizontal movement
        _rigidbody2D.linearVelocity = new Vector2(horizontalInput * moveSpeed, _rigidbody2D.linearVelocity.y);
    }

    private void UpdateAnimation()
    {
        _animator.SetBool("IsGrounded", playerIsGrounded);
        _animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
        _animator.SetFloat("VerticalVelocity", _rigidbody2D.linearVelocity.y);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, groundBoxSize);
    }
}