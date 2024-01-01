//using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class Utils
{
    public static int numberOfPlayers = 0;
    public static int playerOneScore = 0;
    public static int playerTwoScore = 0;
    public static Vector2 GetMousePosition()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return mousePosition;
    }
    public static void InitiateMultiplayer()
    {
        var menuThings = GameObject.FindGameObjectsWithTag("MainMenu");
        foreach (var thing in menuThings)
        {
            Object.Destroy(thing);
        }
    }
}
