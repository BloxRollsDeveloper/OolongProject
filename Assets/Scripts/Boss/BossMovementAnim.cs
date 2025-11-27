using UnityEngine;

[CreateAssetMenu(menuName = ("My Scrubs/boss/animation"))]
public class BossMovementAnim : ScriptableObject
{
    public Vector2[] MovementPoints;
    public bool[] WindUpToggle;
    [Range(0f, 2f)]
    public float[] TelegraphTime;
    public bool[] MovementEaseInToggle;
    //public bool[] Overshoot
    //test
    //player follow
    //random pos

}
