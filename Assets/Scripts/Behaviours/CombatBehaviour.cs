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

    public void AddWeapon(Weapon weapon)
    {
        _weapons.Add(weapon);
        _currentWeapon = weapon;
        weapon.SetWeaponOwner(this);
    }
}
