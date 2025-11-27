using UnityEngine;

[CreateAssetMenu(menuName = "My Scrubs/Data/PlayerData")]
public class ScrubsPlayerData : ScriptableObject
{
    public Vector3 playerCurrentPos;
    public int playerCurrentHealth;
    public HealthStates healthState;
    
        public enum HealthStates
    {
        Hale,
        Injured,
        Bleeding,
        Broken,
    }
}
