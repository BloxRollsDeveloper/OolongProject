using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class HandScript : MonoBehaviour
{
    [Header("projectiles")]
    public GameObject trashBall;

   
    public GameObject carProjectile;
    public RainTestScript rainManager;
    public AttackIndicator flashIndicator;
    public GameObject bomb;
    public GameObject laserObject;
    private Animator _animator;
    private LineRenderer _lineRenderer;
    
    [Header("movement")]
    public Transform stageEdgeTransform;
    private Vector2 _stageEdge;
    private Vector2 _startPos;
    public Transform TrashPickupTransform;
    private Vector2 _TrashPickup; 
    public Rigidbody2D handRb;
    public Collider2D handCollider;
    public Collider2D damageCollider;
    public BossHead bossHead;

    public Vector2 TargetPos;
    
    public float transitionSpeed;
    public bool transitionMove;
    
    
    public bool attacking;
    public bool followPlayer;
    public bool invert;
   

    private float telegraphTimer;
    public float flashDuration;
    public AnimationCurve FlashCurve;

    public int attackChainLocal;
    
    [Header("SFX")]
    [SerializeField] AudioClip[] slamSFXclip;
    [SerializeField] AudioClip[] sweepSFXclip;
    [SerializeField] AudioClip rumbleClip;
    [SerializeField] AudioClip[] grabCrunchClip;

    
    //sine wave math
    [Header("Animation")]
    private float _sinCenterY;
    [SerializeField] float _sinTimer;
    public float amplitude;
    public float frequency;
    
    public AnimationCurve curve;
    private Vector2 zero = Vector2.zero;

    private float speedie = 0.2f;
    public bool easeIn;

    [Header("Boss tests")] 
    public bool attackSlamRandom;
    public bool attackSlamPlayer;
    public bool attackSweep;
    //todo: public bool PlatformDestruction 

    public bool projectileRain;
    public bool projectilePitch;
    public bool projectileBasketball;
    //todo: public bool projectileBomb;

    //todo: public bool laserHorizontal;
    public bool laserHorizontalRandom;
    //todo: public bool laserVertical;
    public bool laserVerticalRandom;
    
    private void Start()
    {
        trashBall.SetActive(false);
        bossHead = GameObject.FindGameObjectsWithTag("Boss")[0].GetComponent<BossHead>();
        telegraphTimer = bossHead.telegraphTime;
        handCollider = GetComponent<Collider2D>();  
        _startPos = transform.position;
        handRb = GetComponent<Rigidbody2D>();
        _stageEdge = stageEdgeTransform.position;
        _TrashPickup = TrashPickupTransform.position;
        _animator = GetComponent<Animator>();
        _lineRenderer = GetComponent<LineRenderer>();
    }
    
    

    private void Update()
    {
        if (bossHead.HardMode)
        {
            damageCollider = GetComponent<Collider2D>(); //disable collider issues
        }


        if (!bossHead.bossActive) return;
        if (transitionMove) //used for attacking animations to smooth transition between 2 points
        {
            if (followPlayer) TargetPos.x = bossHead.PlayerRB.position.x;
            if (easeIn)
            {
                speedie += 100f;
                transform.position = Vector2.MoveTowards(transform.position, TargetPos, speedie*curve.Evaluate(Time.deltaTime));
            }
            else
            {
                transform.position = Vector2.Lerp(transform.position, TargetPos, transitionSpeed * Time.deltaTime);
            }
           
        }
       
        
        if (!attacking) //idle hand animation
        {
            _sinTimer += Time.deltaTime; //sine wave timer
            Vector2 position = transform.position;  //local variable: position
            float sin = Mathf.Sin(_sinTimer*frequency) * amplitude; //sine wave math
            position.y = _startPos.y + sin; //setting positions y to sine output plus the gameobjects y position
            transform.position = position;
        }
        
        //TODO: TEST SUITE
        if (attackSweep) AttackHandSweep();
        if (attackSlamRandom) AttackHandSlamRand();
        if (attackSlamPlayer) attackHandSlamPlayer();
        if (projectileRain) AttackProjectileRain();
        if (projectilePitch) StartCoroutine(AttackProjectilePitch());
        if (projectileBasketball) StartCoroutine(AttackProjectileBasketball());
        if (laserHorizontalRandom) StartCoroutine(AttackLaserHorizontalRandom());
        if (laserVerticalRandom) StartCoroutine(AttackLaserVerticalRandom());
        
        /*
         //todo: ALL OF THIS SHIT
        if (projectileBomb);
        if (laserHorizontalRandom);
        if (laserVertical);
        if (laserVerticalRandom);
        */
        UpdateAnimation();
    }

    public void InitiateAttack()
    {
        if (attacking) return;
        bossHead.AttackPool--;
        var attack = Random.Range(1, 8);
        print("commiting attack nr " + attack);
        
        if (attack == 1) attackSweep = true;
        if (attack == 2) attackSlamRandom = true;
        if (attack == 3) attackSlamPlayer = true;
        if (attack == 4) projectileRain = true;
        if (attack == 5)  projectilePitch = true;
        if (attack == 6) projectileBasketball = true;
        if (attack == 7) laserVerticalRandom = true;
        if (attack == 8) laserHorizontalRandom = true;

    }
    

    private void UpdateAnimation()
    {
        /* todo
        _animator.Play("boss grab");
        _animator.Play("boss palm horizontal")
        _animator.Play("boss palm vertical");
        _animator.Play("Boss fist");
        */
    }
    
    //ATTACK EVENTS
    
    public IEnumerator ResetPosition()  //resets all variables and positions to standard settings
    {
        if (invert) transform.localScale = new Vector2(1, 1);
        else transform.localScale = new Vector2(-1, 1);
        
        damageCollider.enabled = false;
        handCollider.enabled = false;
        TargetPos = _startPos;
        _animator.Play("Boss fist");
        yield return new WaitForSeconds(1f);
        transform.position = _startPos;
        _sinTimer = 0;
        speedie = 0.2f;
        attacking = false;
        transitionMove = false;
    }
    public void AttackHandSlamRand()
    {
        attackSlamRandom = false;
        if (attacking)  return;
        if (attackChainLocal < 1) attackChainLocal = 3;
        if (attackChainLocal > 0) attackChainLocal--;
        transitionMove = true; attacking = true;
        
        _animator.Play("Boss fist");
        TargetPos = new  Vector2(Random.Range(-6,6), 4); //move hand to random position over the stage
        Invoke("AttackHandSlamRand1", bossHead.telegraphTime/2);
    }
    private void AttackHandSlamRand1()
    {
        TargetPos += new Vector2(0,1);  //wind up position
        Invoke("AttackHandSlamRand2", bossHead.telegraphTime/2);
    }
    private void AttackHandSlamRand2() //slam position + sfx
    {
        damageCollider.enabled = true;
        TargetPos = new Vector2(transform.position.x, -3.5f); 
        easeIn = true;
        SoundFXManager.instance.PlayRandomSoundFXClip(slamSFXclip, transform, 1f);
        Invoke("AttackHandSlamRand3", bossHead.telegraphTime);
    }
    private void AttackHandSlamRand3()
    {
        easeIn = false;

        if (attackChainLocal > 0) //chain attacks multiple times if local chain is greater than 0
        {
            damageCollider.enabled = false;
            attacking = false;
            AttackHandSlamRand();
        }else StartCoroutine(ResetPosition());
    }

    public void attackHandSlamPlayer()
    {
        if (attackChainLocal > 0) attackChainLocal--;
        attackSlamPlayer = false;
        if (attacking)  return;
        transitionMove = true; attacking = true;
        
        _animator.Play("Boss fist");
        TargetPos = new  Vector2(bossHead.PlayerRB.transform.position.x, 4); //move hand to top and follow player
        followPlayer = true;
        Invoke("attackHandSlamPlayer1", bossHead.telegraphTime*2);
    }
    private void attackHandSlamPlayer1()
    {
        damageCollider.enabled = true;
        followPlayer = false;
        TargetPos += new Vector2(0,1);  //wind up position
        Invoke("attackHandSlamPlayer2", bossHead.telegraphTime/2);
    }
    private void attackHandSlamPlayer2() //slam position + sfx
    {
        TargetPos = new Vector2(transform.position.x, -3.5f); 
        easeIn = true;

        SoundFXManager.instance.PlayRandomSoundFXClip(slamSFXclip, transform, 1f);
        Invoke("attackHandSlamPlayer3", bossHead.telegraphTime*2);
    }
    private void attackHandSlamPlayer3()
    {
        easeIn = false;

        if (attackChainLocal > 0) //chain attacks multiple times if local chain is greater than 0
        {
            attacking = false;
            attackHandSlamPlayer();
        }
        else
        {
            StartCoroutine(ResetPosition());
        }
    }
    

    public void AttackHandSweep()
    {
        attackSweep = false;
        if (attacking)  return;
        transitionMove = true; attacking = true; handCollider.enabled = true;
        TargetPos = _stageEdge; //move to the stage edge
        flashIndicator.flash = true;
        Invoke("AttackHandSweep1", bossHead.telegraphTime);
    }
    private void AttackHandSweep1() //wind up position
    {
        
        damageCollider.enabled = true;
        _animator.Play("Boss fist");
        TargetPos += new Vector2(1,0);  
        SoundFXManager.instance.PlayRandomSoundFXClip(sweepSFXclip, transform, 1f);
        Invoke("AttackHandSweep2", bossHead.telegraphTime/2);
    }

    private void AttackHandSweep2()
    {
        TargetPos = _stageEdge * new Vector2(-1,1); //sweep position
        Invoke("AttackHandSweep3", 0.5f);
    }
    private void AttackHandSweep3()
    {
        TargetPos = new Vector2(transform.position.x, transform.position.y+3); //fist raise position
        Invoke("AttackHandSweep4", bossHead.telegraphTime/2);
    }
    private void AttackHandSweep4()
    {
        StartCoroutine(ResetPosition());
    }

    public void AttackProjectileRain() //go to trash pickup point
    {
        projectileRain = false;
        if (attacking)  return;
        attacking = true; transitionMove = true;
        
        _animator.Play("boss palm vertical");
        TargetPos = _TrashPickup;   
        //transform.rotation = Quaternion.Euler(0,0,90);
        Invoke("AttackProjectileRain1", bossHead.telegraphTime);
    }
    private void AttackProjectileRain1() //grab trash
    {
        
        _animator.Play("boss grab");
        SoundFXManager.instance.PlayRandomSoundFXClip(grabCrunchClip, transform, 0.5f);
        transform.localScale = transform.localScale * new Vector2(-1, 1);
        trashBall.SetActive(enabled);
        Invoke("AttackProjectileRain2", bossHead.telegraphTime/2);
    }
    private void AttackProjectileRain2() //go to start point and reset position
    {
        
        TargetPos = _startPos; 
        //transform.rotation = Quaternion.Euler(0,0,0);
        Invoke("AttackProjectileRain3", bossHead.telegraphTime/2);
    }
    private void AttackProjectileRain3() //wind up position
    {
        TargetPos += new Vector2(0, -0.5f); 
        transform.rotation = Quaternion.Euler(0,0,180);
        Invoke("AttackProjectileRain4", bossHead.telegraphTime/2);
    }
    private void AttackProjectileRain4() //throwing position
    {
        TargetPos += new Vector2(0,5); 
        easeIn = true;
        Invoke("AttackProjectileRain5", bossHead.telegraphTime/4);
    }
    private void AttackProjectileRain5() //release trash
    {
        trashBall.SetActive(false);
        _animator.Play("boss palm vertical");
        Invoke("AttackProjectileRain6", bossHead.telegraphTime/4);
    }
    private void AttackProjectileRain6() //spawn trash rain
    {
        easeIn = false;
        rainManager.SpawnRain(rumbleClip);
        
        
        Invoke("AttackProjectileRain7", bossHead.telegraphTime/2);
    }
    private void AttackProjectileRain7() //reset position
    {
        transform.localScale = transform.localScale * new Vector2(-1, 1);
        transform.rotation = Quaternion.Euler(0,0,0);
        StartCoroutine(ResetPosition());
    }
    
    public IEnumerator AttackProjectilePitch() //FUCK THIS ATTACK
    {
        projectilePitch = false;
        if (attacking)  yield break;
        attacking = true; transitionMove = true;
        
        TargetPos = _TrashPickup;   //go to trash pickup point
        _animator.Play("boss palm vertical");
        if (invert) transform.localScale *= new Vector2(-1, 1);
        yield return new WaitForSeconds(bossHead.telegraphTime);
        
        _animator.Play("boss grab");
        SoundFXManager.instance.PlayRandomSoundFXClip(grabCrunchClip, transform, 0.5f);
        
        var projectileClone = Instantiate(carProjectile, transform.position, transform.rotation);
        projectileClone.TryGetComponent(out BigProjectileAttack projectileScript);
        projectileClone.TryGetComponent(out Rigidbody2D rb2D);
        projectileScript.spawnerObject = gameObject.transform;
        projectileScript.followSpawner = true;
        projectileScript.basketball = true;
        if (invert) projectileScript.invert = true;
        
        yield return new WaitForSeconds(bossHead.telegraphTime/2);
        
        TargetPos = new Vector2(_TrashPickup.x,Random.Range(-2,4)); //go to random vertical position
        if (invert) transform.localScale = new Vector2(1, -1);
        transform.rotation = Quaternion.Euler(0,0,-90);
        yield return new WaitForSeconds(bossHead.telegraphTime/2);
        
        if (invert) TargetPos -= new Vector2(0.5f, 0); //wind up position
        else TargetPos += new Vector2(0.5f, 0); //wind up position
        yield return new WaitForSeconds(bossHead.telegraphTime/2);
        easeIn = true;
        if (invert) TargetPos -= new Vector2(-2.5f,0); //throwing position
        else TargetPos += new Vector2(-2.5f,0); //throwing position
        transform.rotation = Quaternion.Euler(0,0,0);
        if (invert) transform.localScale*= new Vector2(-1, -1);
        
        _animator.Play("boss palm horizontal");
        yield return new WaitForSeconds(bossHead.telegraphTime/4);

        projectileScript.followSpawner = false; //ball stops following hand
        if (invert) projectileClone.transform.localScale = new Vector3(1, 1, 1); //point ball left
        else projectileClone.transform.localScale = new Vector3(-1, 1, 1);
        projectileScript.FUCKINGMOVE();
        rb2D.gravityScale = 1;
        Destroy(projectileClone, 5);
        easeIn = false;
        
        yield return new WaitForSeconds(bossHead.telegraphTime/2);
        transform.localScale = transform.localScale * new Vector2(-1, 1);
        StartCoroutine(ResetPosition());
    }

    /*
    public void AttackProjectilePitch()
    {
        projectilePitch = false;
        if (attacking)  yield break;
        attacking = true; transitionMove = true;
        
        TargetPos = _TrashPickup;   //go to trash pickup point
        _animator.Play("boss palm vertical");
        if (invert) transform.localScale *= new Vector2(-1, 1);
    }
    private void AttackProjectilePitch1()
    {
        _animator.Play("boss grab");
        todo: problem point
        var projectileClone = Instantiate(carProjectile, transform.position, transform.rotation);
        projectileClone.TryGetComponent(out BigProjectileAttack projectileScript);
        projectileClone.TryGetComponent(out Rigidbody2D rb2D);
        projectileScript.spawnerObject = gameObject.transform;
        projectileScript.followSpawner = true;
        projectileScript.basketball = true;
        if (invert) projectileScript.invert = true;
        
        yield return new WaitForSeconds(bossHead.telegraphTime/2);
    }
    private void AttackProjectilePitch2()
    {
        TargetPos = new Vector2(_TrashPickup.x,Random.Range(-2,4)); //go to random vertical position
        if (invert) transform.localScale = new Vector2(1, -1);
        transform.rotation = Quaternion.Euler(0,0,-90);
        yield return new WaitForSeconds(bossHead.telegraphTime/2);
    }
    private void AttackProjectilePitch3()
    {
                
        if (invert) TargetPos -= new Vector2(0.5f, 0); //wind up position
        else TargetPos += new Vector2(0.5f, 0); //wind up position
        yield return new WaitForSeconds(bossHead.telegraphTime/2);
    }
    private void AttackProjectilePitch4()
    {
        easeIn = true;
        if (invert) TargetPos -= new Vector2(-2.5f,0); //throwing position
        else TargetPos += new Vector2(-2.5f,0); //throwing position
        transform.rotation = Quaternion.Euler(0,0,0);
        if (invert) transform.localScale*= new Vector2(-1, -1);
        
        _animator.Play("boss palm horizontal");
        yield return new WaitForSeconds(bossHead.telegraphTime/4);
    }
    private void AttackProjectilePitch5()
    {
        todo: problem point
        projectileScript.followSpawner = false; //ball stops following hand
        if (invert) projectileClone.transform.localScale = new Vector3(1, 1, 1); //point ball left
        else projectileClone.transform.localScale = new Vector3(-1, 1, 1);
        projectileScript.FUCKINGMOVE();
        rb2D.gravityScale = 1;
        Destroy(projectileClone, 5);
        easeIn = false;
        
        yield return new WaitForSeconds(bossHead.telegraphTime/2);
    }
    private void AttackProjectilePitch6()
    {
        transform.localScale = transform.localScale * new Vector2(-1, 1);
        StartCoroutine(ResetPosition());
    }
    */
    public IEnumerator AttackProjectileBasketball()
    {
        projectileBasketball = false;
        if (attacking)  yield break;
        attacking = true; transitionMove = true;
        
        if (invert) transform.localScale *= new Vector2(-1, 1);
        TargetPos = _TrashPickup;   //go to trash pickup point
        _animator.Play("boss palm vertical");
        yield return new WaitForSeconds(bossHead.telegraphTime);
        
        _animator.Play("boss grab");
        SoundFXManager.instance.PlayRandomSoundFXClip(grabCrunchClip, transform, 0.5f);
        
        var projectileClone = Instantiate(carProjectile, transform.position, transform.rotation);
        projectileClone.TryGetComponent(out BigProjectileAttack projectileScript);
        projectileClone.TryGetComponent(out Rigidbody2D rb2D);
        projectileScript.spawnerObject = gameObject.transform;
        projectileScript.followSpawner = true;
        projectileScript.basketball = true;
        if (invert) projectileScript.invert = true;
        
        yield return new WaitForSeconds(bossHead.telegraphTime/2);
        
        TargetPos = _startPos;
        yield return new WaitForSeconds(bossHead.telegraphTime/2);
        
        TargetPos += new Vector2(0, -2f); //wind up position
        transform.rotation = Quaternion.Euler(0,0,180);
        yield return new WaitForSeconds(bossHead.telegraphTime/2);
        easeIn = true;
        TargetPos += new Vector2(0,7); //throwing position
        yield return new WaitForSeconds(bossHead.telegraphTime/4);
        
        projectileScript.followSpawner = false; //ball stops following hand
        if (invert) projectileClone.transform.localScale = new Vector3(1, 1, 1); //point ball right
        else projectileClone.transform.localScale = new Vector3(-1,1,1); //point ball left
        
        projectileScript.FUCKINGMOVE();
        rb2D.gravityScale = 1;
        Destroy(projectileClone, 5);
        easeIn = false;
        _animator.Play("boss palm vertical");
        
        
        yield return new WaitForSeconds(bossHead.telegraphTime/2);
        transform.rotation = Quaternion.Euler(0,0,0);
        StartCoroutine(ResetPosition());
    }

    public IEnumerator AttackLaserHorizontalRandom()
    {
        
        laserHorizontalRandom = false;
        if (attacking)  yield break;
        if (attackChainLocal < 1) attackChainLocal = 3;
        if (attackChainLocal > 0) attackChainLocal--;
        attacking = true; transitionMove = true;
        
        TargetPos = new Vector2(_stageEdge.x,Random.Range(-2,4)); //go to random vertical position
        _animator.Play("boss palm horizontal");
        transform.localScale *= new Vector2(-1, 1);
        
        yield return new WaitForSeconds(bossHead.telegraphTime/2);
        var projectileClone = Instantiate(laserObject, transform.position, transform.rotation);
        projectileClone.TryGetComponent(out Laser laserScript);
        laserScript.laserFirePoint.transform.position = new Vector3(transform.position.x*-2,transform.position.y,0);
        
        easeIn = true;
        TargetPos += new Vector2(0.1f, 0); //wind up position
        yield return new WaitForSeconds(bossHead.telegraphTime/2);
        easeIn = false;
        TargetPos -= new Vector2(0.1f, 0);
        laserScript.stopLaser(); //plays laser stop animation
        
        if (attackChainLocal > 0) //chain attacks multiple times if local chain is greater than 0
        {
            transform.localScale *= new Vector2(-1, 1);
            attacking = false;
            StartCoroutine(AttackLaserHorizontalRandom());
        }
        else
        {
            StartCoroutine(ResetPosition());
        }
    }

    private IEnumerator LaserFlash() //laser indicator
    {
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, new Vector3(transform.position.x,-4.5f,0));
        

        float elapsedTime = 0f;

        while (elapsedTime < flashDuration)
        {
            elapsedTime += Time.deltaTime;
            float strength = FlashCurve.Evaluate(elapsedTime / flashDuration);
            _lineRenderer.startColor = new Color(
                _lineRenderer.startColor.r, 
                _lineRenderer.startColor.g, 
                _lineRenderer.startColor.b, 
                strength);
            _lineRenderer.endColor = _lineRenderer.startColor;
            yield return null;
        }
    }
    
    public IEnumerator AttackLaserVerticalRandom()
    {
        
        laserVerticalRandom = false;
        if (attacking)  yield break;
        if (attackChainLocal < 1) attackChainLocal = 3;
        if (attackChainLocal > 0) attackChainLocal--;
        attacking = true; transitionMove = true;
        
        TargetPos = new Vector2(Random.Range(-6.5f,6.5f),2); //go to random vertical position
        easeIn = true;
        _animator.Play("boss palm vertical");
        transform.localScale *= new Vector2(-1, 1);
        
        yield return new WaitForSeconds(bossHead.telegraphTime/2);
        StartCoroutine(LaserFlash());
        yield return new WaitForSeconds(bossHead.telegraphTime/2);
        var projectileClone = Instantiate(laserObject, transform.position, transform.rotation);
        projectileClone.TryGetComponent(out Laser laserScript);
        laserScript.laserFirePoint.transform.position = new Vector3(transform.position.x,-4.5f,0); //set laser target position
        
        
        //TargetPos += new Vector2(0, 0.5f); //wind up position
        yield return new WaitForSeconds(bossHead.telegraphTime);
        easeIn = false;
        //TargetPos -= new Vector2(0, -0.5f);
        laserScript.stopLaser(); //plays laser stop animation
        
        if (attackChainLocal > 0) //chain attacks multiple times if local chain is greater than 0
        {
            transform.localScale *= new Vector2(-1, 1);
            attacking = false;
            StartCoroutine(AttackLaserVerticalRandom());
        }
        else
        {
            StartCoroutine(ResetPosition());
        }
    }
}

