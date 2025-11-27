using System.Collections;
using UnityEngine;
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
    public Transform trashPickupTransform;
    private Vector2 _trashPickup; 
    public Rigidbody2D handRb;
    public Collider2D handCollider;
    public Collider2D damageCollider;
    public BossHead bossHead;

    public Vector2 targetPos;
    
    public float transitionSpeed;
    public bool transitionMove;
    
    
    public bool attacking;
    public bool followPlayer;
    public bool invert;
   

    [SerializeField] private float telegraphTimer;
    public float flashDuration;
    public AnimationCurve flashCurve;
    
    public int attackChainLocal;
    public float movementDeadZone = 1f;
    public float curveEval;
    public float windUpMultiplier;
    
    [Header("SFX")]
    [SerializeField] AudioClip[] slamSFXclip;
    [SerializeField] AudioClip[] sweepSFXclip;
    [SerializeField] AudioClip rumbleClip;
    [SerializeField] AudioClip[] grabCrunchClip;

    
    //sine wave math
    [Header("Animation")]
    private float _sinCenterY;
    private float _sinTimer;
    public float amplitude;
    public float frequency;
    
    public AnimationCurve curve;

    private float _curveSpeed = 0.2f;
    public float curveSpeedAddition;
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
        _trashPickup = trashPickupTransform.position;
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
        //telegraphTimer = bossHead.telegraphTime;
        if (transitionMove) //used for attacking animations to smooth transition between 2 points
        {
            if (followPlayer) targetPos.x = bossHead.PlayerRB.position.x;
            if (easeIn)
            {
                _curveSpeed += curveSpeedAddition;
                curveEval = curve.Evaluate(Time.deltaTime);
                transform.position = Vector2.MoveTowards(transform.position, targetPos, _curveSpeed*transitionSpeed*curveEval);
            }
            else
            {
                transform.position = Vector2.Lerp(transform.position, targetPos, transitionSpeed * Time.deltaTime);
            }
           
        }
       
        
        if (!attacking) //idle hand animation
        {
            _sinTimer += Time.deltaTime; //sine wave timer
            Vector2 position = transform.position;  //local variable: position
            float sin = Mathf.Sin(_sinTimer*frequency) * amplitude; //sine wave math
            position.y = _startPos.y + sin; //setting positions y to sine output plus the gameObjects y position
            transform.position = position;
        }
        
        //TODO: TEST SUITE
        if (attackSweep) StartCoroutine(AttackHandSweep());
        if (attackSlamRandom) StartCoroutine(AttackHandSlamRand());
        if (attackSlamPlayer) StartCoroutine(AttackHandSlamPlayer());
        if (projectileRain) StartCoroutine(AttackProjectileRain());
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
        var attack = Random.Range(1, 9);
        print(gameObject.name + " is commiting attack nr " + attack);
        
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
        Debug.Log("resetting position");
        transform.localScale = invert ? new Vector2(1, 1) : new Vector2(-1, 1);
        
        damageCollider.enabled = false;
        handCollider.enabled = false;
        targetPos = _startPos;
        _animator.Play("Boss fist");
        
        Debug.Log("resetMOVE");
        yield return new WaitUntil(() => Vector2.Distance(transform.position, targetPos) < movementDeadZone);
        transform.position = _startPos;
        _sinTimer = 0;
        _curveSpeed = 0.2f;
        attacking = false;
        transitionMove = false;
        Debug.Log("resetDONE");  
    }


    public IEnumerator Movement()
    {
        yield break;
    }

    public IEnumerator AttackHandSlamRand()
    {
        attackSlamRandom = false;
        if (attacking)  yield break; //stop attack from activating if an attack is in progress
        if (attackChainLocal < 1) attackChainLocal = 3; 
        if (attackChainLocal > 0) attackChainLocal--;
        transitionMove = true; attacking = true;
        _curveSpeed = 0.2f;
        
        _animator.Play("Boss fist");
        targetPos = new  Vector2(Random.Range(-6,6), 4); //move hand to random position over the stage
        yield return new WaitUntil(() => Vector2.Distance(transform.position, targetPos) < movementDeadZone);
        yield return new WaitForSeconds(telegraphTimer/2);
        
        targetPos += new Vector2(0,1*windUpMultiplier);  //wind up position
        yield return new WaitUntil(() => Vector2.Distance(transform.position, targetPos) < movementDeadZone);
        
        damageCollider.enabled = true;
        targetPos = new Vector2(transform.position.x, -3.5f); //drop down position
        easeIn = true;
        SoundFXManager.instance.PlayRandomSoundFXClip(slamSFXclip, transform, 1f);
        yield return new WaitUntil(() => Vector2.Distance(transform.position, targetPos) < movementDeadZone);
        yield return new WaitForSeconds(telegraphTimer/2);
        
        easeIn = false;

        if (attackChainLocal > 0) //chain attacks multiple times if local chain is greater than 0
        {
            damageCollider.enabled = false;
            attacking = false;
            StartCoroutine(AttackHandSlamRand());
        }else StartCoroutine(ResetPosition());
    }
    
    
    public IEnumerator AttackHandSlamPlayer()
    {
     if (attackChainLocal > 0) attackChainLocal--;
        attackSlamPlayer = false;
        if (attacking)  yield break;
        transitionMove = true; attacking = true;
        _curveSpeed = 0.2f;
        
        _animator.Play("Boss fist");
        targetPos = new  Vector2(bossHead.PlayerRB.transform.position.x, 4); //move hand to top and follow player
        followPlayer = true;
        yield return new WaitUntil(() => Vector2.Distance(transform.position, targetPos) < movementDeadZone);
        yield return new WaitForSeconds(telegraphTimer*2);
        
        damageCollider.enabled = true;
        followPlayer = false;
        targetPos += new Vector2(0,1*windUpMultiplier);  //wind up position
        yield return new WaitUntil(() => Vector2.Distance(transform.position, targetPos) < movementDeadZone);
        
        targetPos = new Vector2(transform.position.x, -3.5f); 
        easeIn = true;

        SoundFXManager.instance.PlayRandomSoundFXClip(slamSFXclip, transform, 1f);
        yield return new WaitUntil(() => Vector2.Distance(transform.position, targetPos) < movementDeadZone);
        yield return new WaitForSeconds(telegraphTimer*2);
        
        easeIn = false;

        if (attackChainLocal > 0) //chain attacks multiple times if local chain is greater than 0
        {
            attacking = false;
            StartCoroutine(AttackHandSlamPlayer());
        }
        else
        {
            StartCoroutine(ResetPosition());
        }
    }

    public IEnumerator AttackHandSweep()
    {
        attackSweep = false;
        if (attacking)  yield break;
        transitionMove = true; attacking = true; handCollider.enabled = true;
        targetPos = _stageEdge; //move to the stage edge
        yield return new WaitUntil(() => Vector2.Distance(transform.position, targetPos) < movementDeadZone);
        
        damageCollider.enabled = true;
        _animator.Play("Boss fist");
        targetPos += invert ? new Vector2(-1*windUpMultiplier,0) : new Vector2(1*windUpMultiplier,0); //wind up position
        
        flashIndicator.flash = true;
        yield return new WaitUntil(() => Vector2.Distance(transform.position, targetPos) < movementDeadZone);
        SoundFXManager.instance.PlayRandomSoundFXClip(sweepSFXclip, transform, 1f);
        yield return new WaitForSeconds(telegraphTimer/2);
        
        targetPos = _stageEdge * new Vector2(-1,1); //sweep position
        yield return new WaitUntil(() => Vector2.Distance(transform.position, targetPos) < movementDeadZone);
        
        targetPos = new Vector2(transform.position.x, transform.position.y+3); //fist raise position
        yield return new WaitUntil(() => Vector2.Distance(transform.position, targetPos) < movementDeadZone);
        
        StartCoroutine(ResetPosition());
    }

    public IEnumerator AttackProjectileRain()
    {
        projectileRain = false;
        if (attacking)  yield break;
        _curveSpeed = 0.2f;
        attacking = true; transitionMove = true;
        
        _animator.Play("boss palm vertical");
        targetPos = _trashPickup;   
        //transform.rotation = Quaternion.Euler(0,0,90);
        yield return new WaitUntil(() => Vector2.Distance(transform.position, targetPos) < movementDeadZone);
        
        _animator.Play("boss grab");
        SoundFXManager.instance.PlayRandomSoundFXClip(grabCrunchClip, transform, 0.5f);
        transform.localScale = transform.localScale * new Vector2(-1, 1);
        trashBall.SetActive(enabled);
        yield return new WaitForSeconds(telegraphTimer/2);
        
        targetPos = _startPos; 
        //transform.rotation = Quaternion.Euler(0,0,0);
        yield return new WaitUntil(() => Vector2.Distance(transform.position, targetPos) < movementDeadZone);
        
        targetPos += new Vector2(0, -0.5f); 
        transform.rotation = Quaternion.Euler(0,0,180);
        yield return new WaitUntil(() => Vector2.Distance(transform.position, targetPos) < movementDeadZone);
        
        targetPos += new Vector2(0,5); 
        easeIn = true;
        yield return new WaitForSeconds(telegraphTimer/4);
        
        trashBall.SetActive(false);
        _animator.Play("boss palm vertical");
        yield return new WaitForSeconds(telegraphTimer/4);
        
        easeIn = false;
        rainManager.SpawnRain(rumbleClip);
        
        yield return new WaitForSeconds(telegraphTimer/2);
        
        transform.localScale = transform.localScale * new Vector2(-1, 1);
        transform.rotation = Quaternion.Euler(0,0,0);
        StartCoroutine(ResetPosition());
    }
    
    public IEnumerator AttackProjectilePitch() //FUCK THIS ATTACK
    {
        projectilePitch = false;
        if (attacking)  yield break;
        attacking = true; transitionMove = true; _curveSpeed = 0.2f;
        
        targetPos = _trashPickup;   //go to trash pickup point
        _animator.Play("boss palm vertical");
        transform.localScale *= new Vector2(-1, 1);
        
        yield return new WaitUntil(() => Vector2.Distance(transform.position, targetPos) < movementDeadZone);
        
        _animator.Play("boss grab");
        SoundFXManager.instance.PlayRandomSoundFXClip(grabCrunchClip, transform, 0.5f);
        
        var projectileClone = Instantiate(carProjectile, transform.position, transform.rotation); //spawn trash ball projectile
        projectileClone.TryGetComponent(out BigProjectileAttack projectileScript);
        projectileClone.TryGetComponent(out Rigidbody2D rb2D);
        projectileScript.spawnerObject = gameObject.transform;
        projectileScript.followSpawner = true;
        projectileScript.basketball = true;
        if (invert) projectileScript.invert = true;
        
        yield return new WaitForSeconds(bossHead.telegraphTime/2);
        
        targetPos = new Vector2(_trashPickup.x,Random.Range(-2,4)); //go to random vertical position
        if (invert) transform.localScale = new Vector2(1, -1);
        transform.rotation = Quaternion.Euler(0,0,-90);
        yield return new WaitUntil(() => Vector2.Distance(transform.position, targetPos) < movementDeadZone);
        
        if (invert) targetPos -= new Vector2(0.5f, 0); //wind up position
        else targetPos += new Vector2(0.5f, 0); //wind up position
        yield return new WaitUntil(() => Vector2.Distance(transform.position, targetPos) < movementDeadZone);
        easeIn = true;
        if (invert) targetPos -= new Vector2(-2.5f,0); //throwing position
        else targetPos += new Vector2(-2.5f,0); //throwing position
        transform.rotation = Quaternion.Euler(0,0,0);
        if (invert) transform.localScale*= new Vector2(-1, -1);
        else transform.localScale*= new Vector2(-1, 1);
        
        _animator.Play("boss palm horizontal");
        yield return new WaitUntil(() => Vector2.Distance(transform.position, targetPos) < movementDeadZone);

        projectileScript.followSpawner = false; //ball stops following hand
        projectileClone.transform.localScale = invert ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
        projectileScript.FUCKINGMOVE();
        rb2D.gravityScale = 1;
        Destroy(projectileClone, 5);
        easeIn = false;
        
        yield return new WaitForSeconds(bossHead.telegraphTime/2);
        transform.localScale *= new Vector2(-1, 1);
        StartCoroutine(ResetPosition());
    }
    
    public IEnumerator AttackProjectileBasketball()
    {
        projectileBasketball = false;
        if (attacking)  yield break;
        attacking = true; transitionMove = true; _curveSpeed = 0.2f;
        
        if (invert) transform.localScale *= new Vector2(-1, 1);
        targetPos = _trashPickup;   //go to trash pickup point
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
        
        targetPos = _startPos;
        yield return new WaitForSeconds(bossHead.telegraphTime/2);
        
        targetPos += new Vector2(0, -2f); //wind up position
        transform.rotation = Quaternion.Euler(0,0,180);
        yield return new WaitForSeconds(bossHead.telegraphTime/2);
        easeIn = true;
        targetPos += new Vector2(0,7); //throwing position
        yield return new WaitForSeconds(bossHead.telegraphTime/4);
        
        projectileScript.followSpawner = false; //ball stops following hand
        projectileClone.transform.localScale = invert ? new Vector3(1, 1, 1) : new Vector3(-1,1,1);
        
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
        attacking = true; transitionMove = true; _curveSpeed = 0.2f;
        
        targetPos = new Vector2(_stageEdge.x,Random.Range(-2,4)); //go to random vertical position
        _animator.Play("boss palm horizontal");
        transform.localScale *= new Vector2(-1, 1);
        
        yield return new WaitForSeconds(bossHead.telegraphTime/2);
        var projectileClone = Instantiate(laserObject, transform.position, transform.rotation);
        projectileClone.TryGetComponent(out Laser laserScript);
        laserScript.laserFirePoint.transform.position = new Vector3(transform.position.x*-2,transform.position.y,0);
        
        easeIn = true;
        targetPos += new Vector2(0.1f, 0); //wind up position
        yield return new WaitForSeconds(bossHead.telegraphTime/2);
        easeIn = false;
        targetPos -= new Vector2(0.1f, 0);
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
            float strength = flashCurve.Evaluate(elapsedTime / flashDuration);
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
        attacking = true; transitionMove = true; _curveSpeed = 0.2f;
        
        targetPos = new Vector2(Random.Range(-6.5f,6.5f),2); //go to random vertical position
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