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
        _fuckingMOVE = false;
        followSpawner = true;
        _rb = GetComponent<Rigidbody2D>();
        bossHead = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossHead>();
        moveSpeed = bossHead.projectileSpeed;
    }
    
    void Update()
    {
        if (basketball) moveSpeed =  bossHead.projectileSpeed/2;
        if (_fuckingMOVE && !invert) transform.position -= new Vector3(moveSpeed * Time.deltaTime, 0, 0);
        if (_fuckingMOVE && invert) transform.position += new Vector3(moveSpeed * Time.deltaTime, 0, 0);
        if (followSpawner) transform.position = spawnerObject.position;
    }

    public void FUCKINGMOVE()
    {
        _fuckingMOVE = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        transform.rotation = Quaternion.Euler(0,0,transform.rotation.eulerAngles.z+90);
    }
}
