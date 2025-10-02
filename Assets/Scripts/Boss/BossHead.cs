
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class BossHead : MonoBehaviour
{

    private SpriteRenderer _spriteRenderer;
    public  Rigidbody2D headRB;
    public Rigidbody2D PlayerRB;
    public GameObject BossJaw;
    public SpriteRenderer BossJawSprite;
    public GameObject[] handObject;
    public int bossHealth;
    public float telegraphTime;
    public float projectileSpeed;
    public float projectileDamage; //maybe maybe not

    public float attackCooldown;
    private float _attackTimer;
    public int AttackPool;

    private bool _attacking;
    public bool bossActive;
    public float bossStage;

    
    //sine wave stuff
    private float _sinTimer;
    public float frequency;
    public float amplitude;
    private Vector2 _startPos;


    private void Start()
    {
        handObject = GameObject.FindGameObjectsWithTag("BossHand");
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _startPos = transform.position;
        headRB = GetComponent<Rigidbody2D>();
        PlayerRB = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (bossHealth == 150) bossStage = 2;
        if (!bossActive) return;
        if (bossHealth <= 0)
        {
            bossActive = false;
            Death();
        }
        if (_attackTimer > 0) _attackTimer -= Time.deltaTime;

        if (_attackTimer <= 0)
        {
            if (AttackPool > 0)
            {  
                _attackTimer = 1/bossStage;
                for (int i = 0; i < handObject.Length; i++) //INITITATE ATTACK CODE
                {
                    var hand  = handObject[i];
                    var handScript = hand.GetComponent<HandScript>();
                    handScript.InitiateAttack();
                }
            }
            else
            {
                AttackPool = 5;
                _attackTimer = attackCooldown;
            }
        }

        
            _sinTimer += Time.deltaTime; //sine wave timer
        Vector2 position = transform.position;  //local variable: position
            float sin = Mathf.Sin(_sinTimer*frequency) * amplitude; //sine wave math
            position.y = _startPos.y + sin; //setting positions y to sine output plus the gameobjects y position
            transform.position = position;
    }

    public void Death()
    {
        print("IM FUCKING DEAD DICKHEAD");
    }

    public IEnumerator bossDamage()
    {
        _spriteRenderer.color = Color.red;
        BossJawSprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        _spriteRenderer.color = Color.white;
        BossJawSprite.color = Color.white;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            bossHealth--;
            StartCoroutine(bossDamage());
            Destroy(other.gameObject);
        }
    }
    

}
