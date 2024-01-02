using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrosshairScript : MonoBehaviour
{
    public GameObject player;
    public float crossHairDistance = 4.5f;

    private bool useMouse = false;


    private void Start()
    {
    }
    private void Update()
    {
        if(useMouse)
            transform.position = Utils.GetMousePosition();

    }
    void OnAim(InputValue inputValue)
    {
        var targetDirection = inputValue.Get<Vector2>();
        targetDirection.y = QuantizeAxis(targetDirection.y);
        targetDirection.x = QuantizeAxis(targetDirection.x);

        if (targetDirection.y == 0 && (targetDirection.x == 1 || targetDirection.x == -1))
            targetDirection.y++;


        //if (targetDirection == Vector2.zero) return;

        Vector2 offset = (Vector2)player.transform.position + (targetDirection * crossHairDistance);


        if((Vector2)player.transform.position == offset)
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }
        else
        {
            GetComponent<SpriteRenderer>().enabled = true;
        }

        transform.position = new Vector2(offset.x, offset.y);
        useMouse = false;
    }
    int QuantizeAxis ( float input)
    {
        if (input < -0.3f) return -1;
        if (input > 0.3f) return 1;
        return 0;
    }

    void OnMousePosition(InputValue inputValue)
    {
        GetComponent<SpriteRenderer>().enabled = true;
        Vector2 targetPosition = Utils.GetMousePosition();
        transform.position = targetPosition;
        useMouse = true;
    }
}
