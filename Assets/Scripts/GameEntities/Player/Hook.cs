using UnityEngine;
using UnityEngine.InputSystem;

public class Hook : MonoBehaviour
{
    [SerializeField] private GameObject _impactPrefab;
    [SerializeField] private GameObject  _shootingEffectPrefab;
    [SerializeField] private float _hookForce;
    [SerializeField] private float _hookPullOffset;
    [SerializeField] private float _cameraShake;
    [SerializeField] private float _cameraShakeDuration;
    [SerializeField] private HookTarget _hookTarget;
    [SerializeField] private float _hookShootForce;
    private Player _player;
    private Rigidbody2D _rigidbody;
    private Rigidbody2D _playerRigidbody;
    private float _originalPullOffset;
    private float _offsetChanger;
    private Gamepad _gamepad;
    private Transform _playerTransform;
    private Transform _transform;
    private float _soundTimer = 0; 
    private bool _isHookActive = false;
    private float _hookTimeout = 0.2f; 
    private LineRenderer _lineRenderer;
    private SpriteRenderer _spriteRenderer;
    private CircleCollider2D _collider;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _player = gameObject.GetComponentInParent<Player>();
        _playerRigidbody = _player.GetComponent<Rigidbody2D>();
        _originalPullOffset = _hookPullOffset;
        _gamepad = _player.GetComponent<PlayerInput>().GetDevice<Gamepad>();
        _lineRenderer = GetComponent<LineRenderer>();
        _collider = GetComponent<CircleCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Cache transform
        _playerTransform = _player.transform;
        _transform = transform;

        if (_hookTimeout < 0.2f)
            _hookTimeout += Time.deltaTime;


        if (_soundTimer > 1f)
            _soundTimer = 0; 
        else if(_soundTimer > 0)
            _soundTimer += Time.deltaTime;

        if(_isHookActive)
            RenderRope();

        if(IsHookAttached())
        {
            Vector2 fromPlayerToHook = (Vector2)_transform.position - (Vector2)_playerTransform.position;
            fromPlayerToHook.Normalize();
            float distance = Vector2.Distance(_transform.position, _playerTransform.position);

            if((_hookPullOffset > _originalPullOffset && _offsetChanger < 0) || (_hookPullOffset < _originalPullOffset * 6 && _offsetChanger > 0))
            {
                _hookPullOffset += _offsetChanger * Time.deltaTime * 10;

                if(_soundTimer == 0)  // This is to not spam the audio 
                {
                    GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayElastic();
                    _soundTimer += Time.deltaTime;
                }
            }

            if (distance > _hookPullOffset)
            {
                _playerRigidbody.velocity += fromPlayerToHook * _hookForce * Time.deltaTime;
            }

        }
    }

    void OnMove(InputValue inputValue)
    {
        _offsetChanger = inputValue.Get<Vector2>().y * -1;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Utils.GamepadRumble(_gamepad,0f,1f,0.2f);
        GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayHookHit();
        GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayElastic();
        _rigidbody.bodyType = RigidbodyType2D.Static;
        _hookPullOffset = _originalPullOffset;
        Instantiate(_impactPrefab, _transform.position, _transform.rotation);
        _player.camera.GetComponent<PlayerCamera>().StartShake(_cameraShakeDuration, _cameraShake);
    }

    void RenderRope()
    {
        Vector3[] positions = { _transform.position,  _playerTransform.position };
        _lineRenderer.SetPositions(positions);
    }
    public void PlayShootAnimataion()
    {
        Instantiate(_shootingEffectPrefab, _transform.position, _transform.rotation);
    }

    public bool IsHookAttached()
    {
        return _rigidbody.bodyType == RigidbodyType2D.Static && _isHookActive;
    }
    public void InactivateHook()
    {
        if(IsHookAttached())
            GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayHookRetract();

        _isHookActive = false;
        _lineRenderer.enabled = false;
        _spriteRenderer.enabled = false;
        _collider.enabled = false;
        _rigidbody.bodyType = RigidbodyType2D.Static;
    }

    public void ActivateHook()
    {
        _isHookActive = true;
        _lineRenderer.enabled = true;
        _spriteRenderer.enabled = true;
        _collider.enabled = true;
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        _transform.position = _playerTransform.position;
    }

    void OnHook(InputValue input)
    {
        if(input.Get() == null) 
        {
            // Hook button is being released
            if(IsHookAttached())
                GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayHookRetract();

            // This is the release of the button
            if (_isHookActive)
            {
                if(_hookTimeout < 0.2f) // Hook shooting, stop sound effect
                    GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().StopHookShoot();
                InactivateHook();
            }
            return;
        }
        if(_hookTimeout < 0.2f)
        {
            // Hook on delay, can't shoot again
            GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayHookDelay();
            return;
        }

        // SHOOT HOOK
        Utils.GamepadRumble(_player.Gamepad,0f,0.5f,0.2f);

        // Start hook delay timer
        _hookTimeout = 0;

        GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayHookShoot();
        ActivateHook();

        Vector2 hookTarget = _hookTarget.transform.position;
        if (hookTarget == (Vector2)_transform.position)
        {
            // This means there is no crosshair, so shoot upwards
            hookTarget.y += 4.5f;
            if (_player.IsPlayerLookingToTheRight()) // This can't be the horizontal movement because it should work when the dude is not moving
                hookTarget.x += 4.5f; // Loking to the right
            else
                hookTarget.x -= 4.5f;
        }

        Vector2 fromPlayerToHook = hookTarget - (Vector2)_transform.position ;
        fromPlayerToHook.Normalize();

        // Shoot the hook
        _rigidbody.velocity = fromPlayerToHook * _hookShootForce;
        PlayShootAnimataion();
    }
}
