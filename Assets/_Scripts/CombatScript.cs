using System.Collections.Generic;
using UnityEngine;

public class CombatScript : MonoBehaviour
{
    // If you put this into an object it will turn on combat on it
    public List<Weapon> weapons = new List<Weapon>();
    public Teams team;

    public float MaxHP;
    public float CurrentHP;
    public bool IsAlive;
    public Weapon CurrentWeapon;


    private void Start()
    {
        
    }
    private void Update()
    {
    }
}
