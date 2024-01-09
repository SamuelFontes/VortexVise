using UnityEngine;

public class Map : MonoBehaviour
{
    [field:SerializeField] public string Description { get; private set; }
    [field: SerializeField] public Gamemode Gamemode { get; private set; }
    [field: SerializeField] public Vector2 TopRight { get; private set; }// Used to stop the camera and stuff going outside the map
    [field: SerializeField] public Vector2 BottomLeft {  get; private set; }
}
