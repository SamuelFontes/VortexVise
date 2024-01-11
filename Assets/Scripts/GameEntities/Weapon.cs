using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private GameObject _shootingEffectPrefab;
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
    private LineRenderer _lineRenderer;
    private Transform _transform;
    private bool _isFlipped = false;
    private bool _isAiming = false;

    
    void Start()
    {
        _parentSpriteRenderer = transform.parent.GetChild(0).GetComponent<SpriteRenderer>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        transform.position = _weaponOffset;
        _sound = GetComponent<AudioSource>();
        _timeUntilNextShot = _timeBetweenShots;
        _timeUntilReloadFinishes = _timeToReload;
        _ownerRigidbody = _weaponOwner.GetComponent<Rigidbody2D>();
        _lineRenderer = GetComponent<LineRenderer>();
    }
    void Update()
    {
        _transform = transform;

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

        bullet.Init(BaseDamage, _explosionPrefab, transform.parent.GetComponent<Player>().Team,direction,ProjectileForce);
    }

    void RenderWeapon()
    {
        if(_isFlipped != _parentSpriteRenderer.flipX)
        {
            _spriteRenderer.flipX = _parentSpriteRenderer.flipX;
            _isFlipped = _spriteRenderer.flipX;
            _weaponOffset.x *= -1; // Invert number
            transform.position = transform.parent.transform.position +  _weaponOffset;
        }
        if (_isAiming)
        {
            var x = _transform.position.x;
            if (IsLookingRight())
                x += Range;
            else
                x -= Range;
            Vector3[] positions = { _transform.position,  new Vector3(x,_transform.position.y)};
            _lineRenderer.SetPositions(positions);
        }
    }

    bool IsLookingRight() 
    {
        return _isFlipped;
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
        Instantiate(_shootingEffectPrefab, transform.position, transform.rotation);
    }

    void OnLockAim(InputValue input)
    {
        if (input.Get() == null)
        {
            _isAiming = false;
            _lineRenderer.enabled = false;
        }
        else
        {
            _isAiming = true;
            _lineRenderer.enabled = true;
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
