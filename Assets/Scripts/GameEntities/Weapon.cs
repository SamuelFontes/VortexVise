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
    public float ProjectileForce;
    public Ammo Ammo ;
    private CombatBehaviour _weaponOwner;
    public Projectile Projectile;
    public Vector3 WeaponOffset;
    public GameObject Explosion;

    private SpriteRenderer parentSpriteRenderer;
    private SpriteRenderer spriteRenderer;
    private AudioSource sound;

    
    void Start()
    {
        parentSpriteRenderer = transform.parent.GetChild(0).GetComponent<SpriteRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.position = WeaponOffset;
        sound = GetComponent<AudioSource>();
    }
    void Update()
    {
        RenderWeapon();
    }

    void OnShoot()
    {
        sound.Play();
        var bullet = Instantiate(Projectile, transform.position, Quaternion.identity);
        Vector2 direction;
        if (IsLookingRight())
            direction = Vector2.right;
        else 
            direction = Vector2.left;

        bullet.Init(BaseDamage, Explosion, transform.parent.GetComponent<Player>().Team,direction,ProjectileForce);
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

    bool IsLookingRight() { return isFlipped;}

    public void SetWeaponOwner(CombatBehaviour combatant)
    {
        _weaponOwner = combatant;
    }
}
