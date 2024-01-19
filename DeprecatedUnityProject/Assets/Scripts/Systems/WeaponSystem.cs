using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    [SerializeField] private List<Weapon> _weapons = new List<Weapon>();
    [SerializeField] private List<Weapon> _startingWeaponsDeathMatch = new List<Weapon>();

    public void GetWeaponByName(CombatBehaviour combatant, string weaponName) 
    {
        var weapon = _weapons.Where(_ => _.name == weaponName).OrderBy(m => Guid.NewGuid()).FirstOrDefault();
        AddWeaponToCombatant(combatant, weapon);
    }

    public void GetDefaultWeapons(CombatBehaviour combatant)
    {
        if(GameState.Instance.Gamemode == Gamemode.DeathMatch)
        {
            foreach(var weapon in _startingWeaponsDeathMatch)
            {
                AddWeaponToCombatant(combatant, weapon);
            }
        }
    }

    void AddWeaponToCombatant(CombatBehaviour combatant, Weapon weapon)
    {
        var combatantWeapon = Instantiate(weapon, combatant.transform, worldPositionStays:false);
        combatant.AddWeapon(combatantWeapon);
    }
}
