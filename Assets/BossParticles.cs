using UnityEngine;

public class BossParticles : MonoBehaviour
{
    public Sprite[] sprites;
    private Rigidbody2D BossBulletRB;
    public float BossBulletSpeedMax;
    public float BossBulletSpeedMin;
    [SerializeField] private float rotationTimer;
    private float _rTimer;


    void Start()
    {
        Destroy(gameObject, 3f);
        _rTimer = Random.Range(-0.5f, 0.5f);
        BossBulletRB = GetComponent<Rigidbody2D>();
        BossBulletRB.linearVelocity = Vector2.up * Random.Range(BossBulletSpeedMin,BossBulletSpeedMax);
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
            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + 90);
        }

    }

}
