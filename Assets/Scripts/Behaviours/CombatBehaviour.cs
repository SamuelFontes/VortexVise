using System.Collections.Generic;
using UnityEngine;

public class CombatBehaviour : MonoBehaviour
{
    // If you put this into an object it will turn on combat on it
    public float MaxHP { get; private set; }
    public float CurrentHP {  get; private set; }
    public bool IsAlive {  get; private set; }

    private List<Weapon> _weapons = new List<Weapon>();
    private Weapon _currentWeapon;
    private Teams _team;

    public void AddWeapon(Weapon weapon)
    {
        _weapons.Add(weapon);
        _currentWeapon = weapon;
        weapon.SetWeaponOwner(this);
    }
}
