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
    public Vector3 WeaponOffset;

    private SpriteRenderer parentSpriteRenderer;
    private SpriteRenderer spriteRenderer;

    
    void Start()
    {
        parentSpriteRenderer = transform.parent.GetChild(0).GetComponent<SpriteRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.position = WeaponOffset;
    }
    void Update()
    {
        RenderWeapon();
    }

    bool isFlipped = false;
    void RenderWeapon()
    {
        if(isFlipped != parentSpriteRenderer.flipX)
        {
            spriteRenderer.flipX = parentSpriteRenderer.flipX;
            isFlipped = spriteRenderer.flipX;
            WeaponOffset.x *= -1; // Invert number
            transform.position = transform.parent.transform.position +  WeaponOffset;
        }
    }
}
