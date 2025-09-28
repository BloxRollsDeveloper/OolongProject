using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

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

    private float telegraphTimer;

    [Header("Boss tests")] 
    public bool attackSlamRandom;
    public bool attackSlamPlayer;
    public bool attackSweep;
    
    
    //sine wave math
    private float _sinCenterY;
    [SerializeField] float _sinTimer;
    public float amplitude;
    public float frequency;
    
    
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
            transform.position = Vector2.Lerp(transform.position, TargetPos, transitionSpeed * Time.deltaTime);
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
        if (attackSlamRandom) AttackHandSlamPos();
        if (attackSlamPlayer) AttackHandSlamFollow();
        
    }

    
    
    public IEnumerator ResetPosition()  //resets all variables and positions to standard settings
    {
        handCollider.enabled = false;
        TargetPos = _startPos;
        yield return new WaitForSeconds(1f);
        print(TargetPos);
        print(transform.position);
        transform.position = _startPos; 
        attacking = false;
        transitionMove = false;
    }

    public void AttackHandSlamPos()
    {
        attackSlamRandom = false;
    }

    public void AttackHandSlamFollow()
    {
        attackSlamPlayer = false;
    }

    private IEnumerator AttackHandSweep()   
    {
        attackSweep = false;
        if (attacking)  yield break;
        transitionMove = true; attacking = true; handCollider.enabled = true;
        TargetPos = _stageEdge; //move to the stage edge
        yield return new WaitForSeconds(bossHead.telegraphTime);
        
        TargetPos = _stageEdge + new Vector2(1,0);  //wind up position
        yield return new WaitForSeconds(bossHead.telegraphTime/2);
        
        TargetPos = _stageEdge * new Vector2(-1,1); //sweep position
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => handRb.linearVelocity.magnitude < 0.1);
        
        TargetPos = new Vector2(transform.position.x, transform.position.y+3); //fist raise position
        yield return new WaitForSeconds(bossHead.telegraphTime/2);
        StartCoroutine(ResetPosition());
    }
}
