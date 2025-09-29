using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float coolDown;
    private float cooldownTimer;
    
    public Vector2 groundBoxSize = new Vector2(0.8f, 0.2f);
    public LayerMask whatIsGround;
    public Transform groundCheck;
    public bool playerIsGrounded;
    public GameObject bullet;
    public Transform bulletSpawn;
    private InputManager2 _input;
    
    //TODO: COMMENT YOUR CODE
    //TODO FOR REAL: add variable jump height

    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private SpriteRenderer _sprite;
    private float horizontalInput;


    public float gravCuttoff; //the value the players velocity needs to reach to trigger
    public float gravStrength; //the strength of gravity on falling from a jump
    public float terminalVelocity; //max downward velocity player can travel at

    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
        _input = GetComponent<InputManager2>();
    }

    void Update()
    {
        if (cooldownTimer >= 0)
        {
            cooldownTimer -=  Time.deltaTime;
        }
        if(_rigidbody2D.linearVelocity.y < terminalVelocity) _rigidbody2D.linearVelocityY = terminalVelocity; //terminal velocity code, if players Y is lower than this value, sets it to terminal velocity 
        if (_rigidbody2D.linearVelocityY < gravCuttoff) _rigidbody2D.gravityScale = gravStrength;   //jump cutoff, increase gravity when player reaches peak of their jump
       else _rigidbody2D.gravityScale = 1;  //resets gravity back to 1 if players Y velocity is greater than the cutoff value
        
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
        
        if (_input.Jump && playerIsGrounded)
        {
            _rigidbody2D.linearVelocityY = jumpForce;
        }

        if ((_input.Attack || _input.AttackHeld) && cooldownTimer <= 0)
        {
            cooldownTimer = coolDown;
            Instantiate(bullet, bulletSpawn.position, Quaternion.identity); 
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
        Gizmos.DrawWireCube(groundCheck.position, groundBoxSize);
    }
    
}