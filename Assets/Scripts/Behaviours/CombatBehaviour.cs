using System.Collections.Generic;
using UnityEngine;

public class CombatBehaviour : MonoBehaviour
{
    // If you put this into an object it will turn on combat on it
    [field:SerializeField] public float MaxHP { get; private set; }
    [field:SerializeField] public float CurrentHP {  get; private set; }
    public bool IsAlive {  get; private set; }
    public bool IsImortal {  get; private set; }

    [SerializeField] private List<Weapon> _weapons = new List<Weapon>();
    [SerializeField] private Weapon _currentWeapon;
    [SerializeField] private Team _team;
    private Transform _transform;

    private void Update()
    {
        HandleIfOutsideTheMap();
    }

    public void AddWeapon(Weapon weapon)
    {
        weapon.SetWeaponOwner(this);
        _weapons.Add(weapon);
        _currentWeapon = weapon;
        _transform = transform;
    }

    private void HandleIfOutsideTheMap()
    {
        if (Utils.CheckIfItIsOusideTheMap(_transform))
        {
            CurrentHP -= 10;
            transform.position = Vector2.zero;
        }
    } 
}
