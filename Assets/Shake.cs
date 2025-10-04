using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Shake : MonoBehaviour
{
    public bool start = false;
    public float duration;
    public AnimationCurve strongCurve;
    public AnimationCurve smallCurve;
    public bool StrongShake;

    private void Update()
    {
        if (start)
        {
            start = false;
            StartCoroutine(Shaking());
        }
    }

    public void BossShake()
    {
        StrongShake  = true;
        StartCoroutine(Shaking());
    }

    public void PlayerHitShake()
    {
        StrongShake = false;
        StartCoroutine(Shaking());
    }
    
    IEnumerator Shaking()
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            if (StrongShake)
            {
                float strength = strongCurve.Evaluate(elapsedTime / duration);
                transform.position = startPosition + Random.insideUnitSphere * strength;
            }
            else
            {
                float strength = smallCurve.Evaluate(elapsedTime / duration);
                transform.position = startPosition + Random.insideUnitSphere * strength;
            }
            yield return null;
        }
        
        transform.position = startPosition;
    }
}
