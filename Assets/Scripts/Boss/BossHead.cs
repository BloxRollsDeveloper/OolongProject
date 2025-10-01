
using System;
using UnityEngine;
using UnityEngine.Serialization;

public class BossHead : MonoBehaviour
{
    public  Rigidbody2D headRB;
    public Rigidbody2D PlayerRB;
    public GameObject BossJaw;
    public float bossHealth;
    public float telegraphTime;
    public float projectileSpeed;
    public float projectileDamage; //maybe maybe not

    
    //sine wave stuff
    private float _sinTimer;
    public float frequency;
    public float amplitude;
    private Vector2 _startPos;


    private void Start()
    {
        _startPos = transform.position;
        headRB = GetComponent<Rigidbody2D>();
        PlayerRB = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
            _sinTimer += Time.deltaTime; //sine wave timer
            Vector2 position = transform.position;  //local variable: position
            float sin = Mathf.Sin(_sinTimer*frequency) * amplitude; //sine wave math
            position.y = _startPos.y + sin; //setting positions y to sine output plus the gameobjects y position
            transform.position = position;
    }
}
