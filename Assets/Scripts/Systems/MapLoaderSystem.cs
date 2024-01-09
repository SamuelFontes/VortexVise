using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapLoaderSystem : MonoBehaviour
{
    [SerializeField] private List<Map> _maps = new List<Map>();

    public void LoadRandomMap(Gamemode gamemode)
    {
        var map = _maps.Where(m => m.Gamemode == gamemode).OrderBy(m => Guid.NewGuid()).FirstOrDefault();
        LoadMap(map);
    }

    public void LoadMap(Map map)
    {
        if(GameState.Instance.CurrentMap != null)
            UnityEngine.Object.Destroy(GameState.Instance.CurrentMap);

        Instantiate(map.gameObject);
        GameState.Instance.SetCurrentMap(map);
    }
}
