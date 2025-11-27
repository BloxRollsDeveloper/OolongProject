
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class BossHead : MonoBehaviour
{

    [Header("Dependencies")]
    public BossSoundManager bossSoundManager;
    public Shake cameraShake;
    public MenuCode menuCode;
    private SpriteRenderer _spriteRenderer;
    public  Rigidbody2D headRB;
    public Rigidbody2D PlayerRB;
    public GameObject BossJaw;
    public SpriteRenderer BossJawSprite;
    public GameObject[] handObject;
    public Sprite headActive;
    public Sprite headInactive;

    [Header("SFX")]
    public AudioClip bossRoar;
    public AudioClip bossRoar2;
    
    
    [Header("boss properties")]
    public bool HardMode;
    public int bossHealth;
    public int bossHealthMax;
    public bool attacking;
    public bool bossActive;

    public float attackCooldown;
    private float _attackTimer;
    public int AttackPoolMax;
    public int AttackPool;
    public float HeadCooldown;
    private float _headTimer;

    public bool startAnim;
    public float bossStage;

    [Header("Hand Properties")]
    public float telegraphTime;
    public float projectileSpeed;
    public float projectileDamage; //maybe maybe not
    
    [Header("Sine Wave Stuff")]
    private float _sinTimer;
    public float frequency;
    public float amplitude;
    private Vector2 _startPos;

    private void Start()
    {
        bossActive = false;
        bossSoundManager = GetComponent<BossSoundManager>();
        handObject = GameObject.FindGameObjectsWithTag("BossHand");
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _startPos = transform.position;
        headRB = GetComponent<Rigidbody2D>();
        PlayerRB = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        if (startAnim) startingAnimation();
        else
        {
            bossActive = true;
            SoundFXManager.instance.PlayMusic();
        }
        
        bossHealthMax = bossHealth;
    }

    public void startingAnimation() //inactive head idle
    {
        _spriteRenderer.sprite = headInactive;
        BossJaw.gameObject.transform.position = _startPos;
        Invoke("StartingAnim1", 4);
    }

    private void StartingAnim1() //eyes active
        {
            if (HardMode)
            {
                bossHealth = 400;
                bossHealthMax = 400;
            }
        _spriteRenderer.sprite = headActive;
        Invoke ("startingAnim2",2f);
    }

    private void startingAnim2() //roar
    {
        Roar();
        Invoke("StartBoss",3f);
        Invoke("StartMusic",3f);
    }

    private void Roar()
    {
        cameraShake.BossShake();
        BossJaw.gameObject.transform.position = new Vector2(_startPos.x, _startPos.y-1.5f);
        transform.position = Vector2.Lerp(transform.position, new Vector2(transform.position.x, transform.position.y+0.5f), 0.5f);
        SoundFXManager.instance.PlaySoundFXClip(bossRoar,transform,1f);
    }

    private void StartMusic()
    {
        SoundFXManager.instance.PlayMusic();
    }
    private void StartBoss()
    {
        if (bossHealth == bossHealthMax)
        {
            menuCode.tutorialVisible = true;
            menuCode.ShowTutorial();
        }

        attacking = true;
        bossActive = true;
    }
    
    private void Update()
    {
        if (bossHealth == bossHealthMax/2 && bossStage == 1) //stage 2 code
        {
            attacking = false;
            Roar();
            bossStage = 2;
            Invoke("StartBoss",2f);
        }
        if (!bossActive) return;
        if (bossHealth <= 0)
        {
            bossActive = false;
            attacking = false;  
            Invoke("Death",2);
        }
        if (_attackTimer > 0) _attackTimer -= Time.deltaTime;

        if (_attackTimer <= 0 && attacking)
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
                AttackPool = AttackPoolMax;
                _attackTimer = attackCooldown/bossStage;
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
        BossJaw.gameObject.transform.position = new Vector2(_startPos.x, _startPos.y-1.5f);
        transform.position = new Vector2(transform.position.x, transform.position.y+0.5f);
        SoundFXManager.instance.PlaySoundFXClip(bossRoar2,transform,1f);
        print("IM FUCKING DEAD DICKHEAD");
        Invoke("LoadVictory",3);
    }

    private void LoadVictory()
    {
        SceneManager.LoadScene(1); //victory screen
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

    private void HeadLaserAttack()
    {
        
    }


}
