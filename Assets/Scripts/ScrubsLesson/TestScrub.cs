using UnityEngine;

[CreateAssetMenu(menuName = "My Scrubs/Weapon/MeleeWeapon")]
public class ScriptName : ScriptableObject
{
    public Sprite weaponImage;
    public string weaponName;
    public int damage;
    public Effects effect;
    public WeaponTypes WeaponType;
    public DamageTypes DamageType;
    private float _baseDMG;


    public void ApplyDamage(float health)
    {
        switch (effect)
        {
            case Effects.none:
                health -= _baseDMG;
                break;
            case Effects.bleeding:
                break;
        }
    }
    public enum DamageTypes
    {
        piercing,
        smashing,
        slashing,
    }

    public enum Effects
    {
        none,
        poison,
        bleeding,
        lightning,
        paralysed,
        soapy,
        curse,
        wet,
        petrified,
    }
    
    public enum WeaponTypes
    {
        Falcion,
        hammer,
        dagger,
        longSword,
        ShortSword,
        Halberd, 
    }
}
