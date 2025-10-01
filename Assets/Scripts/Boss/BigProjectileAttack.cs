using UnityEngine;

public class BigProjectileAttack : MonoBehaviour
{
    public BossHead bossHead;
    private Rigidbody2D _rb;
    public Transform spawnerObject;
    public bool followSpawner;
    public bool _fuckingMOVE;
    public bool invert;
    public float moveSpeed;
    public bool basketball;
    
    void Start()
    {
        moveSpeed = bossHead.projectileSpeed;
        _fuckingMOVE = false;
        followSpawner = true;
        _rb = GetComponent<Rigidbody2D>();
        bossHead = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossHead>();
    }
    
    void Update()
    {
        if (basketball) moveSpeed =  bossHead.projectileSpeed/2;
        if (_fuckingMOVE && !invert) transform.position -= new Vector3(bossHead.projectileSpeed * Time.deltaTime, 0, 0);
        if (_fuckingMOVE && invert) transform.position += new Vector3(bossHead.projectileSpeed * Time.deltaTime, 0, 0);
        if (followSpawner) transform.position = spawnerObject.position;
    }

    public void FUCKINGMOVE()
    {
        _fuckingMOVE = true;
    }
}
