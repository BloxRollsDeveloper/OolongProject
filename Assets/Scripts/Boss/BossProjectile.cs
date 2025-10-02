using System.Collections;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public Sprite[] sprites;
    private Rigidbody2D BossBulletRB;
    public float BossBulletSpeed;
    [SerializeField] private float rotationTimer;
    private float _rTimer;
    [SerializeField] private AudioClip[] _impactClip;
    
    
    void Start()
    {
        _rTimer = Random.Range(-0.5f, 0.5f);
        BossBulletRB = GetComponent<Rigidbody2D>();
        BossBulletRB.linearVelocity = Vector2.down*BossBulletSpeed;
        gameObject.GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
    }
    
    void Update()
    {
        if (_rTimer > 0)
        {
            _rTimer -= Time.deltaTime;
        }
        else
        {
            _rTimer = rotationTimer;
            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z+90);
        }

    }
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"));
        {
            //damage player
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            SoundFXManager.instance.PlayRandomSoundFXClip(_impactClip, transform, 1f);
            Destroy(gameObject);
        }
        
    }
}
