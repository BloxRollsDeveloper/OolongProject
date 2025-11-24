using System;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthManager : MonoBehaviour
{

    public Image HealthBar;
    public BossHead bossHead;

    private void Update()
    {
        float healthFloat = bossHead.bossHealth;
        float healthMaxFloat = bossHead.bossHealthMax;
        HealthBar.fillAmount = healthFloat / healthMaxFloat;
    }
    

}
