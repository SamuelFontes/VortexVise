using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    public List<GameObject> maps = new List<GameObject>();

    public void LoadRandomMap(Gamemode gamemode)
    {
        var map = maps.Where(m => m.GetComponent<Map>().gamemode == gamemode).OrderBy(m => Guid.NewGuid()).FirstOrDefault();
        LoadMap(map.GetComponent<Map>());
    }

    public void LoadMap(Map map)
    {
        if(GameState.Instance.CurrentMap != null)
            UnityEngine.Object.Destroy(GameState.Instance.CurrentMap);

        Instantiate(map.gameObject);
        GameState.Instance.SetCurrentMap(map.GetComponent<Map>());
    }
}
