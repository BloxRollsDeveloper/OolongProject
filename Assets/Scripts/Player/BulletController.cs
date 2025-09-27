using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    public float despawnTimer = 5f;
    public Rigidbody2D _rigidbody2D; 
    void Start()
    {
        _rigidbody2D.linearVelocity = new Vector2(0f, 1f) * speed;
        Destroy(gameObject, despawnTimer);
        _rigidbody2D.linearVelocity = new Vector2(0f, 1f) * speed;
    }
}