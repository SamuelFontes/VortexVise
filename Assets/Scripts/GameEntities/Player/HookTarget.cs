using UnityEngine;
using UnityEngine.InputSystem;

public class HookTarget : MonoBehaviour
{
    public GameObject player;
    public float crossHairDistance = 4.5f;

    private bool useMouse = false;
    public bool aiming = false;


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
        if (!aiming)
        {
            var targetDirection = inputValue.Get<Vector2>();

            // This is for using the hook, only aim in 8ish directions
            targetDirection.y = QuantizeAxis(targetDirection.y);
            targetDirection.x = QuantizeAxis(targetDirection.x);


            // When moving foward it will hook upwards
            if (targetDirection.y == 0 && (targetDirection.x == 1 || targetDirection.x == -1))
                targetDirection.y++;

            Vector2 offset = (Vector2)player.transform.position + (targetDirection * crossHairDistance);


            transform.position = new Vector2(offset.x, offset.y);
        }
    }
    int QuantizeAxis ( float input)
    {
        if (input < -0.5f) return -1;
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
