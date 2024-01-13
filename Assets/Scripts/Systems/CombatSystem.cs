using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    private List<CombatBehaviour> _combatants = new List<CombatBehaviour>();
    private WeaponSystem _weaponSystem;

    private void Start()
    {
        _weaponSystem = GetComponent<WeaponSystem>();
    }

    // This should spawn, deactive when killed, and activate entities on respawn
    public void ProcessGameMode()
    {
        switch (GameState.Instance.Gamemode)
        {
            case Gamemode.DeathMatch:
            {
                ProcessDeathMatch();
                break;
            }
            case Gamemode.Mission:
            {

                break;
            }

        }

    }
    void ProcessDeathMatch()
    {
        var deadPlayers = GetDeadEntities();
        for(int i = 0; i < deadPlayers.Count; i++)
        {
            deadPlayers[i].gameObject.SetActive(false);
            StartCoroutine(Respawn(deadPlayers[i], 3f));// TODO: set respawn timer to use current game configuration
        }
    }


    List<CombatBehaviour> GetDeadEntities()
    {
        var deadThings = _combatants.Where(c => !c.IsAlive && c.gameObject.activeSelf).ToList();
        return deadThings;
    }

    IEnumerator Respawn(CombatBehaviour combatant, float respawnDelay) {
        float timer = 0;
 
        while (timer < respawnDelay) {
            timer += Time.deltaTime;
 
            yield return null;
        }
        combatant.gameObject.SetActive(true);
        combatant.ResetCombatant();
        _weaponSystem.GetDefaultWeapons(combatant);
    }
    public void AddCombatant(CombatBehaviour combatant)
    {
        _combatants.Add(combatant);
    }
    public void RemoveCombatant(CombatBehaviour combatant)
    {
        _combatants.Remove(combatant);
    }
}
