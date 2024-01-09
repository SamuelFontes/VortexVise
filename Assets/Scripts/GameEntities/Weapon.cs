using UnityEngine;

public class Weapon : MonoBehaviour
{
    [field: SerializeField] public string Name { get; private set; }
    [field:SerializeField] public string Description { get; private set; }
    [field: SerializeField] public float Range { get; private set; }
    [field:SerializeField] public float BaseDamage { get; private set; }
    [field:SerializeField] public float SelfKnockback { get; private set; }
    [field:SerializeField] public bool HasLaser { get; private set; }
    [field:SerializeField] public int MaxAmmo { get; private set; }
    [field:SerializeField] public int CurrentAmmo { get; private set; }
    [field:SerializeField] public float ProjectileForce { get; private set; }
    [field:SerializeField] public Ammo AmmoType  { get; private set; }

    [SerializeField] private Projectile _projectile;
    [SerializeField] private Vector3 _weaponOffset;
    [SerializeField] private GameObject _explosion;
    private SpriteRenderer _parentSpriteRenderer;
    private SpriteRenderer _spriteRenderer;
    private AudioSource _sound;
    private CombatBehaviour _weaponOwner;

    
    void Start()
    {
        _parentSpriteRenderer = transform.parent.GetChild(0).GetComponent<SpriteRenderer>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        transform.position = _weaponOffset;
        _sound = GetComponent<AudioSource>();
    }
    void Update()
    {
        RenderWeapon();
    }

    void OnShoot()
    {
        _sound.Play();
        var bullet = Instantiate(_projectile, transform.position, Quaternion.identity);
        Vector2 direction;
        if (IsLookingRight())
            direction = Vector2.right;
        else 
            direction = Vector2.left;

        bullet.Init(BaseDamage, _explosion, transform.parent.GetComponent<Player>().Team,direction,ProjectileForce);
    }

    bool isFlipped = false;
    void RenderWeapon()
    {
        if(isFlipped != _parentSpriteRenderer.flipX)
        {
            _spriteRenderer.flipX = _parentSpriteRenderer.flipX;
            isFlipped = _spriteRenderer.flipX;
            _weaponOffset.x *= -1; // Invert number
            transform.position = transform.parent.transform.position +  _weaponOffset;
        }
    }

    bool IsLookingRight() 
    {
        return isFlipped;
    }

    public void SetWeaponOwner(CombatBehaviour combatant)
    {
        _weaponOwner = combatant;
    }
}
