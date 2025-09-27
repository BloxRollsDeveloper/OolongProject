using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    public float bulletCooldown = 3f;
    
    public float despawnTimer = 5f;
    public Rigidbody2D _rigidbody2D; 
    void Start()
    {
        _rigidbody2D.linearVelocity = new Vector2(0f, 1f) * speed;
        Destroy(gameObject, despawnTimer);
        //Add BulletCooldown at Void Start (WIP)
    }
}