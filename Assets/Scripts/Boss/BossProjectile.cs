using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    private Rigidbody2D BossBulletRB;
    public float BossBulletSpeed;
    void Start()
    {
        BossBulletRB = GetComponent<Rigidbody2D>();
        BossBulletRB.linearVelocity = Vector2.down*BossBulletSpeed;
    }
    
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"));
        {
            //damage player
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground")) Destroy(gameObject);
    }
}
