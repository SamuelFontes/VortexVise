using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public string Id;
    public Team Team { get; set; }

    [SerializeField] private Hook _hook;
    public PlayerCamera camera;
    public GameObject skin;

    // Setup player movement
    public float jumpForce = 25;
    public float moveSpeed = 80;
    public float maxMoveSpeed = 20;
    Rigidbody2D playerRigidbody;
    SpriteRenderer spriteRenderer;
    public Gamepad Gamepad { get; private set; }
    TrailRenderer trailRenderer;
    AudioSource windSound;


    float horizontalMovement = 0;

    bool canDoubleJump = true;

    void Start()
    {
        Id = GetInstanceID().ToString();
        playerRigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = skin.GetComponent<SpriteRenderer>();
        Gamepad = GetComponent<PlayerInput>().GetDevice<Gamepad>();
        trailRenderer = GetComponent<TrailRenderer>();
        windSound = GetComponent<AudioSource>();
    }

    void Update()
    {
        Move();

        Animate();
        ProcessPlayerVelocityEffects();

        ProcessDoubleJump(false);

    }

    void OnJump()
    {
        if (playerRigidbody.velocity.y == 0 || _hook.IsHookAttached())
        {
            if (_hook.IsHookAttached())
            {
                if (IsPlayerLookingToTheRight())
                    playerRigidbody.velocity += Vector2.right * jumpForce;
                else
                    playerRigidbody.velocity += Vector2.left * jumpForce;
                ProcessDoubleJump(true);
            }
            _hook.InactivateHook();
            GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayJump();
            playerRigidbody.velocity += Vector2.up * jumpForce;
        }
        else if (canDoubleJump)
        {
            GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayJump();
            playerRigidbody.velocity += Vector2.up * jumpForce;
            ProcessDoubleJump(true);
        }
    }

    void OnMove(InputValue inputValue)
    {
        horizontalMovement = inputValue.Get<Vector2>().x;
    }


    void OnShoot(InputValue inputValue)
    {
    }

    void Move()
    {
        if (playerRigidbody.velocity.x < maxMoveSpeed && playerRigidbody.velocity.x > maxMoveSpeed * -1)
        {
            if (horizontalMovement > 0)
            {
                spriteRenderer.flipX = true;
                playerRigidbody.velocity += Vector2.right * (moveSpeed * Time.deltaTime * horizontalMovement);
            }
            else if (horizontalMovement < 0)
            {
                spriteRenderer.flipX = false;
                playerRigidbody.velocity += Vector2.left * (moveSpeed * Time.deltaTime * (horizontalMovement * -1));
            }
        }
    }

    float animationTimer = 0f;
    int animationState = 0;
    void Animate()
    {
        // TODO: make this avaliable to every entity 
        if (animationTimer > 0.1f)
        {
            if (animationState == 0)
            {
                skin.transform.Rotate(new Vector3(0, 0, 9));
                skin.transform.localPosition = skin.transform.localPosition + Vector3.up * 0.1f;
                animationState = 1;
            }
            else if (animationState == 1)
            {
                skin.transform.localRotation = Quaternion.identity;
                skin.transform.localPosition = Vector3.zero;
                animationState = 2;
            }
            else if (animationState == 2)
            {
                skin.transform.localPosition = skin.transform.localPosition + Vector3.up * 0.1f;
                skin.transform.Rotate(new Vector3(0, 0, -9));
                animationState = 3;
            }
            else if (animationState == 3)
            {
                skin.transform.localRotation = Quaternion.identity;
                skin.transform.localPosition = Vector3.zero;
                animationState = 0;
            }
            animationTimer = 0f;
        }

        if (horizontalMovement != 0)
        {
            animationTimer += Time.deltaTime;
        }
        else if (doubleJumpRotationAmount == 0)
        {
            animationTimer = 0;
            skin.transform.Rotate(new Vector3(0, 0, 0));
            animationState = 0;
            skin.transform.localRotation = Quaternion.identity;
            skin.transform.localPosition = Vector3.zero;
        }
    }

    float doubleJumpTimer = 0f;
    float doubleJumpRotationAmount = 0f;
    float doubleJumpRotationForce;

    void ProcessDoubleJump(bool playerDoubleJumped)
    {
        var baseForce = 1125;
        if (playerDoubleJumped)
        {
            canDoubleJump = false;
            if (IsPlayerLookingToTheRight())
                doubleJumpRotationForce = baseForce * -1;
            else
                doubleJumpRotationForce = baseForce;
            doubleJumpRotationAmount += Time.deltaTime;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.8f);
            trailRenderer.startColor = Color.black;
            trailRenderer.endColor = Color.black;
            //TODO: IMPORTANT: make player invecible while rotating
        }

        if (doubleJumpRotationAmount != 0f)
        {
            var rotationAmount = doubleJumpRotationForce * Time.deltaTime;
            doubleJumpRotationAmount += rotationAmount;
            skin.transform.Rotate(new Vector3(0, 0, rotationAmount));
        }
        if (doubleJumpRotationAmount > 360f || doubleJumpRotationAmount < -360f)
        {
            skin.transform.localRotation = Quaternion.identity;
            skin.transform.localPosition = Vector3.zero;
            doubleJumpRotationAmount = 0f;
            trailRenderer.startColor = Color.white;
            trailRenderer.endColor = Color.white;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
        }

        if (!canDoubleJump && (playerRigidbody.velocity.y == 0 || _hook.IsHookAttached()))
            doubleJumpTimer += Time.deltaTime;

        if (doubleJumpTimer > 0.1f)
        {
            canDoubleJump = true;
            doubleJumpTimer = 0;
        }
    }

    public bool IsPlayerLookingToTheRight()
    {
        // This just makes the code more readable
        return spriteRenderer.flipX;
    }

    float lastVelocity = 0f;
    void ProcessPlayerVelocityEffects()
    {
        Vector2 velocity = playerRigidbody.velocity;
        var effectPower = velocity.magnitude / 100;
        // Trail
        if (velocity.magnitude > 30)
        {
            trailRenderer.time = effectPower;
        }
        else if (trailRenderer.time > 0)
        {
            trailRenderer.time -= Time.deltaTime;
        }

        if (lastVelocity - velocity.magnitude > 40)
        {
            // If the stop is too fast the dude hit his head into something
            GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayHighSpeedHit();
        }
        lastVelocity = velocity.magnitude;
        // Wind sound
        if (velocity.magnitude < 30 && windSound.volume > 0)
            windSound.volume -= Time.deltaTime;
        else
            windSound.volume += Time.deltaTime;

    }
}
