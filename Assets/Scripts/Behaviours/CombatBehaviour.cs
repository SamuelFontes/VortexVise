using System.Collections.Generic;
using UnityEngine;

public class CombatBehaviour : MonoBehaviour
{
    // If you put this into an object it will turn on combat on it
    [field:SerializeField] public float MaxHP { get; private set; }
    [field:SerializeField] public float CurrentHP {  get; private set; }
    [field:SerializeField] public float Defense {  get; private set; }
    public bool IsAlive { get; private set; } = true;
    public bool IsDead { get; private set; } = false;
    public CombatantType Type { get; private set; }

    [SerializeField] private List<Weapon> _weapons = new List<Weapon>();
    [SerializeField] private Weapon _currentWeapon;
    [SerializeField] private Team _team;
    [SerializeField] private GameObject _damagePrefab;
    [SerializeField] private GameObject _deathPrefab;
    private Transform _transform;
    private Rigidbody2D _rigidbody;
    private Player _player;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        _transform = transform;
        HandleIfOutsideTheMap();
    }

    public void AddWeapon(Weapon weapon)
    {
        weapon.SetWeaponOwner(this);
        _weapons.Add(weapon);
        _currentWeapon = weapon;
    }

    private void HandleIfOutsideTheMap()
    {
        if (Utils.CheckIfItIsOusideTheMap(_transform))
        {
            CurrentHP -= 10;
            transform.position = Vector2.zero;
            CheckIfDied();
        }
    } 

    public void ApplyDamage(float damage)
    {
        // TODO: Think of a better damaging system
        if(Defense >= damage)
        {
            damage = 1;
        }
        damage -= Defense;
        CurrentHP -= damage;
        Instantiate(_damagePrefab, _transform.position, _transform.rotation);
        CheckIfDied();
    }

    void CheckIfDied()
    {
        if(CurrentHP < 0)
        {
            ProcessDeath();
        }
    }

    void ProcessDeath()
    {
        IsAlive = false;
        Instantiate(_deathPrefab, _transform.position, _transform.rotation);
        foreach(var weapon in _weapons)
        {
            GameObject.Destroy(weapon.gameObject);
        }
        _weapons.Clear();
        _currentWeapon = null;
        if(_player != null)
        {
            _player.SetAsDeadOrAlive(false);
        }
    }
    public void SetAsDead()
    {
        IsDead = true;
    }

    public void ResetCombatant()
    {
        CurrentHP = MaxHP;
        IsAlive = true;
        IsDead = false;
        // TODO: After implementing spawnzone choose one and respawn the entity there
        _transform.position = Vector3.zero; 
        _rigidbody.velocity = Vector3.zero;
        // TODO: Setup weapons here
        if(_player != null)
        {
            _player.ResetPlayer();
        }
    }
}
