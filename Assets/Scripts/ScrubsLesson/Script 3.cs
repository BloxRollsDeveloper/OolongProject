using System;
using UnityEngine;

public class Script3 : MonoBehaviour
{
    public ScrubsPlayerData data;


    public void Update()
    {
        if (data.playerCurrentHealth >= 100)
        {
            print("YOURE GONNA FUCKING DIE");
        }
        
    }
}
