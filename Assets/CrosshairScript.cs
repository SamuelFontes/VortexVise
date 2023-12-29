using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairScript : MonoBehaviour
{
    public GameObject player;
    public float crossHairDistance = 4.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 targetPosition = Utils.GetMousePosition(); // FIXME: get player input instead of mouse

        // Get crosshair direction
        Vector2 targetDiretion = targetPosition - (Vector2)player.transform.position;
        targetDiretion.Normalize();

        Vector2 offset = (Vector2)player.transform.position + targetDiretion * crossHairDistance;
        //offset.Normalize();

        transform.position = new Vector2(offset.x, offset.y);

    }
}
