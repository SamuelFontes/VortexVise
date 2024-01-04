using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string Name;
    public string Description;
    public float Range;
    public float AimRange;
    public float BaseDamage;  
    public float TargetKnockback;
    public float SelfKnockback;
    public bool HasScope;
    public int MaxAmmo;
    public int CurrentAmmo;
    public Ammo Ammo ;
    public GameObject WeaponOwner;
    public Vector2 WeaponOffset;

    private void Start()
    {
    }
    private void Update()
    {
    }
}
