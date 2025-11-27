using UnityEngine;

public class PlayerScrub : MonoBehaviour
{
    public ScrubsPlayerData data;
    public int health;
    
    private void Update()
    {
        data.playerCurrentPos = transform.position;
        data.playerCurrentHealth = health;

        if (health >= 70)
        {
            data.healthState = ScrubsPlayerData.HealthStates.Hale;
        }
        else if (health < 70 && health >= 50)
        {
            data.healthState = ScrubsPlayerData.HealthStates.Injured;
        }
        else if (health < 50 && health >= 30)
        {
            data.healthState = ScrubsPlayerData.HealthStates.Bleeding;
        }
        else if (health < 30)
        {
            data.healthState = ScrubsPlayerData.HealthStates.Broken;
        }
    }

#if UNITY_6000_0_OR_NEWER
    //editor only code
#endif
}
