using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class HandScript : MonoBehaviour
{
    public Rigidbody2D handRb;
    public Collider2D handCollider;
    public BossHead bossHead;

    private Vector2 _startPos;
    public Vector2 _stageEdge;

    public bool transitionMove;
    public Vector2 TargetPos;
    public float transitionSpeed;
    
    
    public bool attacking;
    public bool followPlayer;

    private float telegraphTimer;

    public int attackChainLocal;
    public int attackChainGlobal;

    [Header("Boss tests")] 
    public bool attackSlamRandom;
    public bool attackSlamPlayer;
    public bool attackSweep;
    
    
    //sine wave math
    private float _sinCenterY;
    [SerializeField] float _sinTimer;
    public float amplitude;
    public float frequency;
    
    public AnimationCurve curve;
    private Vector2 zero = Vector2.zero;

    private float speedie = 0.2f;
    public bool easeIn;
    
    
    private void Start()
    {
        bossHead = GameObject.FindGameObjectsWithTag("Boss")[0].GetComponent<BossHead>();
        telegraphTimer = bossHead.telegraphTime;
        handCollider = GetComponent<Collider2D>();  
        _startPos = transform.position;
        handRb = GetComponent<Rigidbody2D>();
        _stageEdge = GameObject.Find("Stage Edge Right").GetComponent<Vector2>();
    }
    
    

    private void Update()
    {
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
        
        if (attackSweep) StartCoroutine(AttackHandSweep());
        if (attackSlamRandom) StartCoroutine(AttackHandSlamPos());
        if (attackSlamPlayer) StartCoroutine(AttackHandSlamFollow());
        
    }

    
    
    public IEnumerator ResetPosition()  //resets all variables and positions to standard settings
    {
        handCollider.enabled = false;
        TargetPos = _startPos;
        yield return new WaitForSeconds(1f);
        print(TargetPos);
        print(transform.position);
        transform.position = _startPos;
        _sinTimer = 0;
        speedie = 0.2f;
        attacking = false;
        transitionMove = false;
    }

    public IEnumerator AttackHandSlamPos() //attack with a hand slam in random positions
    {
        attackSlamRandom = false;
        if (attacking)  yield break;
        if (attackChainLocal < 1) attackChainLocal = 3;
        if (attackChainLocal > 0) attackChainLocal--;
        transitionMove = true; attacking = true;
        
        
        TargetPos = new  Vector2(Random.Range(-6,6), 4); //move hand to random position over the stage
        yield return new WaitForSeconds(bossHead.telegraphTime/2);
        
        TargetPos += new Vector2(0,1);  //wind up position
        yield return new WaitForSeconds(bossHead.telegraphTime/2);
        
        TargetPos = new Vector2(transform.position.x, -3.5f); //slam position
        easeIn = true;
        yield return new WaitForSeconds(bossHead.telegraphTime);
        easeIn = false;

        if (attackChainLocal > 0) //chain attacks multiple times if local chain is greater than 0
        {
            attacking = false;
            StartCoroutine(AttackHandSlamPos());
        }else StartCoroutine(ResetPosition());
        
    }

    public IEnumerator AttackHandSlamFollow() //hand slam attack following player
    {
        if (attackChainLocal > 0) attackChainLocal--;
        attackSlamPlayer = false;
        if (attacking)  yield break;
        transitionMove = true; attacking = true;
        
        TargetPos = new  Vector2(bossHead.PlayerRB.transform.position.x, 4); //move hand to top and follow player
        followPlayer = true;
        yield return new WaitForSeconds(bossHead.telegraphTime*3);
        
        followPlayer = false;
        TargetPos += new Vector2(0,1);  //wind up position
        yield return new WaitForSeconds(bossHead.telegraphTime/2);
        
        TargetPos = new Vector2(transform.position.x, -3.5f); //slam position
        easeIn = true;
        yield return new WaitForSeconds(bossHead.telegraphTime*2);
        easeIn = false;

        if (attackChainLocal > 0) //chain attacks multiple times if local chain is greater than 0
        {
            attacking = false;
            StartCoroutine(AttackHandSlamFollow());
        }else StartCoroutine(ResetPosition());
    }

    public IEnumerator AttackHandSweep() //hand sweep attack
    {
        attackSweep = false;
        if (attacking)  yield break;
        transitionMove = true; attacking = true; handCollider.enabled = true;
        TargetPos = _stageEdge; //move to the stage edge
        yield return new WaitForSeconds(bossHead.telegraphTime);
        
        TargetPos += new Vector2(1,0);  //wind up position
        yield return new WaitForSeconds(bossHead.telegraphTime/2);
        
        TargetPos = _stageEdge * new Vector2(-1,1); //sweep position
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => handRb.linearVelocity.magnitude < 0.1);
        
        TargetPos = new Vector2(transform.position.x, transform.position.y+3); //fist raise position
        yield return new WaitForSeconds(bossHead.telegraphTime/2);
        StartCoroutine(ResetPosition());
    }
}
