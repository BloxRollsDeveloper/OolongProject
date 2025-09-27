using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpSpeed = 7f;

    public bool playerIsGrounded;
    public Transform groundCheck;
    public LayerMask whatIsGround;
    public Vector2 groundBoxSize = new Vector2(0.8f, 0.2f);

    private InputManager2 _input;
    private Rigidbody2D _rigidbody2D;

    private void Start()
    {
        _input = GetComponent<InputManager2>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Check if touching ground
        playerIsGrounded = Physics2D.OverlapBox(groundCheck.position, groundBoxSize, 0f, whatIsGround);

        // Jump
        if (_input.Jump && playerIsGrounded)
        {
            Vector2 vel = _rigidbody2D.linearVelocity;
            vel.y = jumpSpeed;
            _rigidbody2D.linearVelocity = vel;
        }
    }

    private void FixedUpdate()
    {
        // Horizontal movement
        Vector2 vel = _rigidbody2D.linearVelocity;
        vel.x = _input.Horizontal * moveSpeed;
        _rigidbody2D.linearVelocity = vel;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundCheck.position, groundBoxSize);
    }
}