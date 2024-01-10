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
    [SerializeField] private float _timeBetweenShots;
    [SerializeField] private float _timeToReload;
    private float _timeUntilNextShot;
    private float _timeUntilReloadFinishes;
    private SpriteRenderer _parentSpriteRenderer;
    private SpriteRenderer _spriteRenderer;
    private AudioSource _sound;
    private CombatBehaviour _weaponOwner;
    private Rigidbody2D _ownerRigidbody;
    private Player _player;

    
    void Start()
    {
        _parentSpriteRenderer = transform.parent.GetChild(0).GetComponent<SpriteRenderer>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        transform.position = _weaponOffset;
        _sound = GetComponent<AudioSource>();
        _timeUntilNextShot = _timeBetweenShots;
        _timeUntilReloadFinishes = _timeToReload;
        _ownerRigidbody = _weaponOwner.GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if(_timeUntilNextShot < _timeBetweenShots)
            _timeUntilNextShot += Time.deltaTime;
        if(_timeUntilReloadFinishes < _timeToReload)
            _timeUntilReloadFinishes += Time.deltaTime;

        RenderWeapon();
    }

    void OnShoot()
    {
        if (_timeUntilNextShot < _timeBetweenShots)
            return;
        if (_timeUntilReloadFinishes < _timeToReload)
            return;
        if(CurrentAmmo <= 0)
        {
            GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayHookDelay();
            return;
        }
        _timeUntilNextShot = 0;
        CurrentAmmo--;
        ApplyShootEffects();
        ApplySelfKnockback();
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
        _player = _weaponOwner.gameObject.GetComponent<Player>();
    }

    private void ApplyShootEffects()
    {
        _sound.Play();
        if (_player != null)
        {
            Utils.GamepadRumble(_player.Gamepad, 1f, 1f, 0.3f);
            _player.Camera.StartShake(0.1f,0.1f);
        }
    }

    private void OnReload()
    {
        if (_timeUntilReloadFinishes < _timeToReload || CurrentAmmo == MaxAmmo)
            return;
        // TODO: create reload bar with timing minigame
        GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayReload();
        CurrentAmmo = MaxAmmo;
        _timeUntilReloadFinishes = 0;
    }


    private void ApplySelfKnockback() 
    {
        if (IsLookingRight())
            _ownerRigidbody.velocity += Vector2.left * SelfKnockback;
        else 
            _ownerRigidbody.velocity += Vector2.right * SelfKnockback;
    }
}
