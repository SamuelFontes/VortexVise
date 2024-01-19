using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapLoaderSystem : MonoBehaviour
{
    public static MapLoaderSystem Instance { get; private set; }
    [SerializeField] private List<Map> _maps = new List<Map>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public void LoadRandomMap(Gamemode gamemode)
    {
        var map = _maps.Where(m => m.Gamemode == gamemode).OrderBy(m => Guid.NewGuid()).FirstOrDefault();
        LoadMap(map);
    }

    public void LoadMap(Map map)
    {
        var currentMap = GameState.Instance.CurrentMap;
        if(currentMap != null)
            GameObject.Destroy(currentMap.gameObject);

        var newMap = Instantiate(map);
        GameState.Instance.SetCurrentMap(newMap);
    }

    public List<Map> GetMapList(Gamemode gamemode)
    {
        var maps = _maps.Where(m => m.Gamemode == gamemode).ToList();
        return maps;
    }
}
