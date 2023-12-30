using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrosshairScript : MonoBehaviour
{
    public GameObject player;
    public float crossHairDistance = 4.5f;

    private bool useMouse = false;


    private void Update()
    {
        if(useMouse)
            transform.position = Utils.GetMousePosition();

    }
    void OnAim(InputValue inputValue)
    {
        var targetDirection = inputValue.Get<Vector2>();
        Vector2 offset = (Vector2)player.transform.position + (targetDirection * crossHairDistance);

        transform.position = new Vector2(offset.x, offset.y);
        useMouse = false;
    }

    void OnMousePosition(InputValue inputValue)
    {
        Vector2 targetPosition = Utils.GetMousePosition();
        transform.position = targetPosition;
        useMouse = true;
    }
}
