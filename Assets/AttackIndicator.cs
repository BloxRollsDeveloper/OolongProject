using System;
using System.Collections;
using UnityEngine;

public class AttackIndicator : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public AnimationCurve FlashCurve;
    public float flashDuration;
    public bool flash;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(
            spriteRenderer.color.r, 
            spriteRenderer.color.g, 
            spriteRenderer.color.b, 
            0);
    }

    private void Update()
    {
        if (flash)
        {
            flash =  false;
            StartCoroutine(FlashSweep());
        }
    }
    
    
    IEnumerator FlashSweep()
    {
        gameObject.SetActive(true);

        float elapsedTime = 0f;

        while (elapsedTime < flashDuration)
        {
            elapsedTime += Time.deltaTime;
            float strength = FlashCurve.Evaluate(elapsedTime / flashDuration);
            spriteRenderer.color = new Color(
                spriteRenderer.color.r, 
                spriteRenderer.color.g, 
                spriteRenderer.color.b, 
                strength);
            yield return null;
        }
    }
}
