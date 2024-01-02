using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    public GameObject hook;
    public GameObject crosshair;
    public GameObject bullet;
    public GameObject camera;
    public float jumpForce = 25;
    public float moveSpeed = 80;
    public float maxMoveSpeed = 20;
    public float hookShootForce = 100;
    public float rocketDelay = 1f;


    private Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;
    private float horizontal = 0;
    private float rocketTimer = 0;
    private bool firingRocket = false;
    private GameObject playerCamera;
    // Start is called before the first frame update
    void Start()
    {
        name = GetInstanceID().ToString();
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        InstantiateCamera();
        var player = new Player(name, playerCamera.name);
        GameLogic.AddLocalPlayer(player);
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        Animate();

        if(firingRocket)
        {
            rocketTimer += Time.deltaTime;
        }
        if(firingRocket && rocketTimer > rocketDelay)
        {
            rocketTimer = 0;
            firingRocket = false;
        }
    }

    private void OnJump()
    {
        // FIXME: It should only allow the player to jump when the hook is attached
        if (rigidbody.velocity.y == 0 || hook.active)
        {
            hook.SetActive(false);
            GameObject.FindWithTag("AudioSystem").GetComponent<AudioScript>().PlayJump();
            rigidbody.velocity += Vector2.up * jumpForce; 
        }
    }

    private void OnMove(InputValue inputValue)
    {
        horizontal = inputValue.Get<Vector2>().x;
    }

    private void OnHook(InputValue input)
    {
        if(input.Get() == null)
        {
            // This is the release of the button
            hook.SetActive(false);
            return;
        }

        GameObject.FindWithTag("AudioSystem").GetComponent<AudioScript>().PlayHookShoot();
        hook.SetActive(true);
        hook.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        hook.transform.position = transform.position;


        Vector2 hookTarget = crosshair.transform.position;
        if (!crosshair.GetComponent<SpriteRenderer>().enabled)
        {
            // This means there is no crosshair, so shoot upwards
            hookTarget.y += 4.5f;
            if (spriteRenderer.flipX)
                hookTarget.x += 4.5f; // Loking to the right
            else
                hookTarget.x -= 4.5f;


        }


        Vector2 fromPlayerToHook = hookTarget - (Vector2)transform.position ;
        fromPlayerToHook.Normalize();

        hook.GetComponent<Rigidbody2D>().velocity = fromPlayerToHook * hookShootForce;
    }

    private void OnShoot(InputValue inputValue) 
    {
        if(firingRocket && rocketTimer < rocketDelay)
        {
            return;
        }
        firingRocket = true;
        //Gamepad.current.SetMotorSpeeds(0.123f, 0.234f);

        GameObject.FindWithTag("AudioSystem").GetComponent<AudioScript>().PlayRocketFire();
        var shoot = Instantiate(bullet, transform.position, crosshair.transform.rotation);
        shoot.layer = gameObject.layer;
        shoot.GetComponent<RocketScript>().gamepad = Gamepad.current;

        Vector2 target = crosshair.transform.position;
        if (!crosshair.GetComponent<SpriteRenderer>().enabled)
        {
            // This means there is no crosshair, so shoot upwards
            target.y += 4.5f;
            if (spriteRenderer.flipX)
                target.x += 4.5f; // Loking to the right
            else
                target.x -= 4.5f;
        }

        Vector2 fromPlayerToTarget = target - (Vector2)transform.position ;
        fromPlayerToTarget.Normalize();

        shoot.GetComponent<Rigidbody2D>().velocity = fromPlayerToTarget * 100;
    }

    private void Move()
    {
        if (rigidbody.velocity.x < maxMoveSpeed && rigidbody.velocity.x > maxMoveSpeed * -1) 
        {
            if(horizontal > 0)
            {
                spriteRenderer.flipX = true;
                rigidbody.velocity += Vector2.right * (moveSpeed * Time.deltaTime * horizontal);
            } else if(horizontal < 0)
            {
                spriteRenderer.flipX = false;
                rigidbody.velocity += Vector2.left * (moveSpeed * Time.deltaTime * (horizontal * -1));
            }
        } 
    } 

    private void Animate()
    {
        // TODO: Make support for multiple skins
        if(horizontal != 0)
        {
            GetComponent<Animator>().Play("FatsoRunning");
        }
        else
        {
            GetComponent<Animator>().Play("FatsoIdle");
        }
    }

    private void InstantiateCamera()
    {
        playerCamera = Instantiate(camera);
        playerCamera.name = GetInstanceID().ToString();
        playerCamera.GetComponent<CameraScript>().target = transform;
    }
}
