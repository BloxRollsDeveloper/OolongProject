
using System;
using UnityEngine;
using UnityEngine.Serialization;

public class BossHead : MonoBehaviour
{
    public  Rigidbody2D headRB;
    public Rigidbody2D PlayerRB;
    public float bossHealth;
    public float telegraphTime;


    private void Start()
    {
        headRB = GetComponent<Rigidbody2D>();
        PlayerRB = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
    }
}
