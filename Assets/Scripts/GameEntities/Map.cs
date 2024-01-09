using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Map : MonoBehaviour
{
    public string Description;
    public Gamemode Gamemode;
    public Vector2 TopRight; // Used to stop the camera and stuff going outside the map
    public Vector2 BottomLeft;
}
