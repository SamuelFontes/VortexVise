using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    public List<GameObject> weapons = new List<GameObject>();
    public void GetWeaponByName(GameObject entity, string weapon) 
    {
        var w = weapons.Where(_ => _.name == weapon).OrderBy(m => Guid.NewGuid()).FirstOrDefault();

        Instantiate(w.gameObject,entity.transform,worldPositionStays:false);
        var ws = w.GetComponent<Weapon>();
        ws.WeaponOwner = entity;
        entity.GetComponent<CombatScript>().CurrentWeapon = ws;
        entity.GetComponent<CombatScript>().weapons.Add(ws);
    }
}
