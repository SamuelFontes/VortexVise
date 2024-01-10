using UnityEngine;
using UnityEngine.InputSystem;

public class HookTarget : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private float _crossHairDistance = 4.5f;
    private bool _useMouse = false;

    private void OnAim(InputValue inputValue)
    {
        var targetDirection = inputValue.Get<Vector2>();

        // This is for using the hook, only aim in 8ish directions
        targetDirection.y = QuantizeAxis(targetDirection.y);
        targetDirection.x = QuantizeAxis(targetDirection.x);

        // When moving foward it will hook upwards
        if (targetDirection.y == 0 && (targetDirection.x == 1 || targetDirection.x == -1))
            targetDirection.y++;

        Vector2 offset = (Vector2)_player.transform.position + (targetDirection * _crossHairDistance);

        transform.position = new Vector2(offset.x, offset.y);
    }

    private int QuantizeAxis ( float input)
    {
        if (input < -0.5f) return -1;
        if (input > 0.3f) return 1;
        return 0;
    }

}
