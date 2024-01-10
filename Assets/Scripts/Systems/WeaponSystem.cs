using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    [SerializeField] private List<Weapon> _weapons = new List<Weapon>();

    public void GetWeaponByName(CombatBehaviour combatant, string weaponName) 
    {
        var weapon = _weapons.Where(_ => _.name == weaponName).OrderBy(m => Guid.NewGuid()).FirstOrDefault();

        var combatantWeapon = Instantiate(weapon, combatant.transform, worldPositionStays:false);
        combatant.AddWeapon(combatantWeapon);
    }
}
