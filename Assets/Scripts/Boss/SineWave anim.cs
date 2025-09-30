using UnityEngine;

public class SineWaveanim : MonoBehaviour
{
    //sine wave stuff
    private float _sinTimer;
    public float frequency;
    public float amplitude;
    public Transform TargetPos;
    public float jawOffset;

    private void Start()
    {
        _sinTimer = 0.5f;
    }

    private void Update()
    {
        _sinTimer -= Time.deltaTime; //sine wave timer
        Vector3 position = transform.position;  //local variable: position
        float sin = Mathf.Sin(_sinTimer*frequency) * amplitude; //sine wave math
        position.y = TargetPos.position.y + sin; //setting positions y to sine output plus the gameobjects y position
        transform.position = position + new Vector3(0,jawOffset,0);
    }
}
