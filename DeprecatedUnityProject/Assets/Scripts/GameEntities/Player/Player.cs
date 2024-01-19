using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{
    public string Id { get; private set; }
    public Team Team { get; private set; }
    public Gamepad Gamepad { get; private set; }
    public PlayerCamera Camera { get; private set; }
    public bool IsAlive { get; private set; } = true;

    [SerializeField] private Hook _hook;
    [SerializeField] private GameObject _skin;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _maxMoveSpeed;
    [SerializeField] private float _bounciness;
    [SerializeField] private float _animationSpeed;
    [SerializeField] private float _baseRotationForce = 650;
    private Rigidbody2D _playerRigidbody;
    private SpriteRenderer _spriteRenderer;
    private TrailRenderer _trailRenderer;
    private AudioSource _windSound;
    private CombatBehaviour _combatBehaviour;
    private float _lastVelocity = 0f;
    private float _doubleJumpTimer = 0f;
    private float _doubleJumpRotationAmount = 0f;
    private float _doubleJumpRotationForce;
    private float _animationTimer = 0f;
    private int _animationState = 0;
    private float _horizontalMovement = 0;
    private bool _canDoubleJump = true;
    private bool _lockAimSide = false;

    void Start()
    {
        Id = GetInstanceID().ToString();
        Gamepad = GetComponent<PlayerInput>().GetDevice<Gamepad>();

        _playerRigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = _skin.GetComponent<SpriteRenderer>();
        _trailRenderer = GetComponent<TrailRenderer>();
        _windSound = GetComponent<AudioSource>();
        _combatBehaviour = GetComponent<CombatBehaviour>();
    }

    void Update()
    {
        if (IsAlive)
            Move();
        Animate();
        ProcessPlayerVelocityEffects();
        ProcessDoubleJump(false);

    }

    void OnJump()
    {
        if (!IsAlive)
            return;
        if (_playerRigidbody.velocity.y == 0 || _hook.IsHookAttached())
        {
            if (_hook.IsHookAttached())
            {
                if (_horizontalMovement > 0 || (_horizontalMovement == 0 && IsPlayerLookingToTheRight()))
                    _playerRigidbody.velocity += Vector2.right * _jumpForce;
                else if(_horizontalMovement < 0|| (_horizontalMovement == 0 && !IsPlayerLookingToTheRight()))
                    _playerRigidbody.velocity += Vector2.left * _jumpForce;
                ProcessDoubleJump(true);
            }
            _hook.InactivateHook();
            GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayJump();
            _playerRigidbody.velocity += Vector2.up * _jumpForce;
        }
        else if (_canDoubleJump)
        {
            GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayJump();
            _playerRigidbody.velocity += Vector2.up * _jumpForce;
            ProcessDoubleJump(true);
        }
    }

    void OnMove(InputValue inputValue)
    {
        _horizontalMovement = inputValue.Get<Vector2>().x;
    }

    void Move()
    {
        if (_horizontalMovement > 0)
        {
            if(!_lockAimSide)
                _spriteRenderer.flipX = true;
            if (_playerRigidbody.velocity.x < _maxMoveSpeed && _playerRigidbody.velocity.x > _maxMoveSpeed * -1)
                _playerRigidbody.velocity += Vector2.right * (_moveSpeed * Time.deltaTime * _horizontalMovement);
        }
        else if (_horizontalMovement < 0)
        {
            if(!_lockAimSide)
                _spriteRenderer.flipX = false;
            if (_playerRigidbody.velocity.x < _maxMoveSpeed && _playerRigidbody.velocity.x > _maxMoveSpeed * -1)
                _playerRigidbody.velocity += Vector2.left * (_moveSpeed * Time.deltaTime * (_horizontalMovement * -1));
        }
    }

    void AnimationBounce()
    {
        var defaultScaleX = 0.5f;
        var defaultScaleY = 0.5f;
        var amount = Time.deltaTime * _bounciness;
        if(_horizontalMovement != 0)
        {
            if (_animationState == 0 || _animationState == 2)
            {
                _skin.transform.localScale += new Vector3(amount,0f);
                if(_skin.transform.localScale.y >= defaultScaleY)
                    _skin.transform.localScale += new Vector3(0,-(amount * 2));
            }
            else if (_animationState == 1 || _animationState == 3)
            {
                if(_skin.transform.localScale.x >= defaultScaleX)
                    _skin.transform.localScale += new Vector3(-(amount * 2),0f);
                _skin.transform.localScale += new Vector3(0,amount);
            }
        }
        else if(_skin.transform.localScale != new Vector3(defaultScaleX,defaultScaleY))
        {
            _skin.transform.localScale = new Vector3(defaultScaleX,defaultScaleY);
        }

    }
    void Animate()
    {
        if (_doubleJumpRotationAmount != 0f)
            return;
        var animationVelocity = 0.1f; 
        if (_playerRigidbody.velocity.magnitude > 20f) animationVelocity = 0.05f;
        // TODO: make this avaliable to every entity 
        if (_animationTimer > animationVelocity)
        {
            if (_animationState == 0)
            {
                _skin.transform.Rotate(new Vector3(0, 0, 9));
                _skin.transform.localPosition = _skin.transform.localPosition + Vector3.up * 0.1f;
                _animationState = 1;
            }
            else if (_animationState == 1)
            {
                _skin.transform.localRotation = Quaternion.identity;
                _skin.transform.localPosition = Vector3.zero;
                _animationState = 2;
            }
            else if (_animationState == 2)
            {
                _skin.transform.localPosition = _skin.transform.localPosition + Vector3.up * 0.1f;
                _skin.transform.Rotate(new Vector3(0, 0, -9));
                _animationState = 3;
            }
            else if (_animationState == 3)
            {
                _skin.transform.localRotation = Quaternion.identity;
                _skin.transform.localPosition = Vector3.zero;
                _animationState = 0;
            }
            _animationTimer = 0f;
        }

        if (_horizontalMovement != 0)
        {
            _animationTimer += Time.deltaTime;
        }
        else if (_doubleJumpRotationAmount == 0)
        {
            _animationTimer = 0;
            _skin.transform.Rotate(new Vector3(0, 0, 0));
            _animationState = 0;
            _skin.transform.localRotation = Quaternion.identity;
            _skin.transform.localPosition = Vector3.zero;
        }
        AnimationBounce();
    }

    void ProcessDoubleJump(bool playerDoubleJumped)
    {
        if (playerDoubleJumped)
        {
            _canDoubleJump = false;
            if (IsPlayerLookingToTheRight())
                _doubleJumpRotationForce = _baseRotationForce * -1;
            else
                _doubleJumpRotationForce = _baseRotationForce;
            _doubleJumpRotationAmount += Time.deltaTime;
            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 0.6f);
            _trailRenderer.startColor = Color.black;
            _trailRenderer.endColor = Color.black;
            // MAKE PLAYER IMMORTAL WHILE ROLLING
            gameObject.layer = Team.GetImmortalTeamLayer();
        }

        if (_doubleJumpRotationAmount != 0f)
        {
            var rotationAmount = _doubleJumpRotationForce * Time.deltaTime;
            _doubleJumpRotationAmount += rotationAmount;
            _skin.transform.Rotate(new Vector3(0, 0, rotationAmount));
        }
        if (_doubleJumpRotationAmount > 360f || _doubleJumpRotationAmount < -360f)
        {
            _skin.transform.localRotation = Quaternion.identity;
            _skin.transform.localPosition = Vector3.zero;
            _doubleJumpRotationAmount = 0f;
            _trailRenderer.startColor = Color.white;
            _trailRenderer.endColor = Color.white;
            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 1f);
            // Make player mortal again
            gameObject.layer = Team.GetTeamLayer();
        }

        if (!_canDoubleJump && (_playerRigidbody.velocity.y == 0 || _hook.IsHookAttached()))
            _doubleJumpTimer += Time.deltaTime;

        if (_doubleJumpTimer > 0.1f)
        {
            _canDoubleJump = true;
            _doubleJumpTimer = 0;
        }
    }

    public bool IsPlayerLookingToTheRight()
    {
        // This just makes the code more readable
        return _spriteRenderer.flipX;
    }

    void ProcessPlayerVelocityEffects()
    {
        Vector2 velocity = _playerRigidbody.velocity;
        var effectPower = velocity.magnitude / 100;
        // Trail
        if (velocity.magnitude > 30)
        {
            _trailRenderer.time = effectPower;
        }
        else if (_trailRenderer.time > 0)
        {
            _trailRenderer.time -= Time.deltaTime;
        }

        if (_lastVelocity - velocity.magnitude > 40)
        {
            // If the stop is too fast the dude hit his head into something
            GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayHighSpeedHit();
        }
        _lastVelocity = velocity.magnitude;
        // Wind sound
        if (velocity.magnitude < 30 && _windSound.volume > 0)
            _windSound.volume -= Time.deltaTime;
        else if(velocity.magnitude > 30)
            _windSound.volume += Time.deltaTime;
    }
    public void SetPlayerCamera(PlayerCamera camera)
    {
        Camera = camera;
    }

    public void SetPlayerTeam(Team team) 
    {
        Team = team;    
        // Set the layer on unity
        gameObject.layer = Team.GetTeamLayer();
    }
    void OnLockAim(InputValue input)
    {
        if (input.Get() == null)
            _lockAimSide = false;
        else
            _lockAimSide = true;
    } 

    public void ResetPlayer()
    {
        _hook.InactivateHook();
        _horizontalMovement = 0;
        _lockAimSide = false;
        SetAsDeadOrAlive(true);
    }

    public void SetAsDeadOrAlive(bool isAlive)
    {
        _spriteRenderer.enabled = isAlive;
        IsAlive = isAlive;
        _hook.enabled = isAlive;
        if (isAlive)
        {
            gameObject.layer = Team.GetTeamLayer();
            _playerRigidbody.bodyType = RigidbodyType2D.Dynamic;
        }
        else
        {
            // Player is dead
            _playerRigidbody.bodyType = RigidbodyType2D.Static;
            gameObject.layer = _hook.gameObject.layer; // HACK: I mean, this will disable the collisions when the player is dead, but if there is any other implementation to the hook this should probably be checked to see if it works properly.
            _hook.InactivateHook();
        }
    }

    void OnPause() 
    {
        UISystem.Instance.ShowHidePauseMenu();
    }

}
