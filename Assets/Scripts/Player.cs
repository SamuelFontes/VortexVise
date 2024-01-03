using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public string Id;
    public GameObject hook;
    public GameObject crosshair;
    public GameObject bullet;
    public GameObject camera;
    public float jumpForce = 25;
    public float moveSpeed = 80;
    public float maxMoveSpeed = 20;
    public float hookShootForce = 100;
    public float rocketDelay = 1f;
    public Teams Team { get; set; } 


    private Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;
    private float horizontal = 0;
    private float rocketTimer = 0;
    private bool firingRocket = false;
    private float hookTimer = 0.2f;
    private GameObject playerCamera;
    private Gamepad gamepad;
    private float rumbleTimer = 0.3f;
    private bool canDoubleJump = true;
    private float doubleJumpTimer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        Id = GetInstanceID().ToString();
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        Animate();

        if (hookTimer < 0.2f)
        {
            hookTimer += Time.deltaTime;
        }
        if (gamepad != null && rumbleTimer >= 0.3f)
        {
            gamepad.SetMotorSpeeds(0,0);
            gamepad = null;
        }
        else
        {
            rumbleTimer += Time.deltaTime;
        }

        if(firingRocket)
        {
            rocketTimer += Time.deltaTime;
        }
        if(firingRocket && rocketTimer > rocketDelay)
        {
            rocketTimer = 0;
            firingRocket = false;
        }
        if (!canDoubleJump && (rigidbody.velocity.y == 0 || (hook.active && hook.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Static))) 
            doubleJumpTimer += Time.deltaTime;

        if(doubleJumpTimer > 1f)
        {
            canDoubleJump = true;
            doubleJumpTimer = 0;
        }
    }

    private void OnJump()
    {
        if (rigidbody.velocity.y == 0 || (hook.active && hook.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Static))
        {
            if(hook.active && hook.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Static)
            {
                GameObject.FindWithTag("AudioSystem").GetComponent<AudioScript>().PlayHookRetract();
                if(horizontal > 0)
                    rigidbody.velocity += Vector2.right * jumpForce; 
                else
                    rigidbody.velocity += Vector2.left * jumpForce; 
            }
            hook.SetActive(false);
            GameObject.FindWithTag("AudioSystem").GetComponent<AudioScript>().PlayJump();
            rigidbody.velocity += Vector2.up * jumpForce; 
        }
        else if (canDoubleJump)
        {
            GameObject.FindWithTag("AudioSystem").GetComponent<AudioScript>().PlayJump();
            rigidbody.velocity += Vector2.up * jumpForce;
            canDoubleJump = false;
        }
    }

    private void OnMove(InputValue inputValue)
    {
        horizontal = inputValue.Get<Vector2>().x;
    }

    private void OnHook(InputValue input)
    {
        if(input.Get() == null) // FIXME: This don't work well with the trigger
        {
            if(hook.active && hook.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Static)
                GameObject.FindWithTag("AudioSystem").GetComponent<AudioScript>().PlayHookRetract();
            // This is the release of the button
            if (hook.active)
            {
                if(hookTimer < 0.2f)
                    GameObject.FindWithTag("AudioSystem").GetComponent<AudioScript>().StopHookShoot();
                hook.SetActive(false);
            }
            return;
        }
        if(hookTimer < 0.2f)
        {
            GameObject.FindWithTag("AudioSystem").GetComponent<AudioScript>().PlayHookDelay();
            return;
        }

        hookTimer = 0;
        gamepad = Gamepad.current;
        if(gamepad != null)
            gamepad.SetMotorSpeeds(0, 0.5f);
        rumbleTimer = 0;



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

    private static void Rumble(float motor, float motor2, float time) 
    {
 
 
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
            //var gun = transform.GetChild(2).gameObject;
            if(horizontal > 0)
            {
                spriteRenderer.flipX = true;
                //gun.GetComponent<SpriteRenderer>().flipX = false;
                //gun.transform.position = new Vector2(transform.position.x+1.6f, gun.transform.position.y);
                rigidbody.velocity += Vector2.right * (moveSpeed * Time.deltaTime * horizontal);
            } else if(horizontal < 0)
            {
                //gun.GetComponent<SpriteRenderer>().flipX = true;
                //gun.transform.position = new Vector2(transform.position.x-1.6f, gun.transform.position.y);
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

}
