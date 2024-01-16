using UnityEngine;
using UnityEngine.InputSystem;

public class HookTarget : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private float _crossHairDistance = 4.5f;
    private bool _isAiming = false;

    private void OnAim(InputValue inputValue)
    {
        if (!_player.IsAlive)
            return;

        Vector2 targetDirection;
        if(inputValue != null) 
            targetDirection = inputValue.Get<Vector2>();
        else
            targetDirection = Vector2.zero;

        // This is for using the hook, only aim in 8ish directions
        targetDirection.y = QuantizeAxis(targetDirection.y);
        targetDirection.x = QuantizeAxis(targetDirection.x);

        // When moving foward it will hook upwards
        if (targetDirection.y == 0 && (targetDirection.x == 1 || targetDirection.x == -1))
            targetDirection.y++;

        if(targetDirection.y == 0 && targetDirection.x == 0 && _isAiming)
        {
            if (_player.IsPlayerLookingToTheRight())
                targetDirection.x += 1;
            else
                targetDirection.x -= 1;
        }
        // this below make upwards have 5 hook directions
        Debug.Log(inputValue.Get<Vector2>());
        if(QuantizeAxis(inputValue.Get<Vector2>().x) > 0 && inputValue.Get<Vector2>().y > 0)
        {
            targetDirection.x -= 0.5f;
        } else if(QuantizeAxis(inputValue.Get<Vector2>().x) < 0 && inputValue.Get<Vector2>().y > 0)
        {
            targetDirection.x += 0.5f;
        }

        Vector2 offset = (Vector2)_player.transform.position + (targetDirection * _crossHairDistance);

        transform.position = new Vector2(offset.x, offset.y);
    }

    private int QuantizeAxis ( float input)
    {
        if (input < -0.5f) return -1;
        if (input > 0.5f) return 1;
        return 0;
    }

    void OnLockAim(InputValue input)
    {
        if (input.Get() == null)
            _isAiming = false;
        else
            _isAiming = true;
        OnAim(null);
    } 

    public float GetDefaultTargetDistance()
    {
        return _crossHairDistance;
    }
}
