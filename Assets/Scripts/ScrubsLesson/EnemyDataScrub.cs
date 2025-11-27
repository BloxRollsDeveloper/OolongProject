using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(menuName = "My Scrubs/Enemies")]
public class EnemyDataScrub : ScriptableObject
{
    public AnimatorController animator;
    public string enemyName;
    public float enemyHealth;

    public EnemyAbilities enemyAbility;

        public class EnemyAbilities
    {
        public void SpewFire()
        {
            // sped fire
        }
    }
}
