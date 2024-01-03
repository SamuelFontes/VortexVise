using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public string Id;
    public Teams Team { get; set; } 

    public GameObject hook;
    public GameObject crosshair;
    public new GameObject camera;
    
    // Setup player movement
    public float jumpForce = 25;
    public float moveSpeed = 80;
    public float maxMoveSpeed = 20;

    public float hookShootForce = 100;
    private float hookTimer = 0.2f; 

    private Rigidbody2D playerRigidbody;
    private Rigidbody2D hookRigidbody;
    private SpriteRenderer spriteRenderer;

    private float horizontalMovement = 0;

    private bool canDoubleJump = true;
    private float doubleJumpTimer = 0f;

    void Start()
    {
        Id = GetInstanceID().ToString();
        playerRigidbody = GetComponent<Rigidbody2D>();
        hookRigidbody = hook.GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Move();

        Animate();

        if (hookTimer < 0.2f) 
            hookTimer += Time.deltaTime; 

        if (!canDoubleJump && (playerRigidbody.velocity.y == 0 || IsHookAttached())) 
            doubleJumpTimer += Time.deltaTime;

        if(doubleJumpTimer > 0.2f)
        {
            canDoubleJump = true;
            doubleJumpTimer = 0;
        }
    }

    private void OnJump()
    {
        if (playerRigidbody.velocity.y == 0 || IsHookAttached())
        {
            if(IsHookAttached())
            {
                GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayHookRetract();
                if(horizontalMovement > 0)
                    playerRigidbody.velocity += Vector2.right * jumpForce; 
                else
                    playerRigidbody.velocity += Vector2.left * jumpForce; 
            }
            hook.SetActive(false);
            GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayJump();
            playerRigidbody.velocity += Vector2.up * jumpForce; 
        }
        else if (canDoubleJump)
        {
            GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayJump();
            playerRigidbody.velocity += Vector2.up * jumpForce;
            canDoubleJump = false;
        }
    }

    private void OnMove(InputValue inputValue)
    {
        horizontalMovement = inputValue.Get<Vector2>().x;
    }

    private void OnHook(InputValue input)
    {
        if(input.Get() == null) 
        {
            // Hook button is being released
            if(IsHookAttached())
                GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayHookRetract();

            // This is the release of the button
            if (hook.activeInHierarchy)
            {
                if(hookTimer < 0.2f) // Hook shooting, stop sound effect
                    GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().StopHookShoot();
                hook.SetActive(false);
            }
            return;
        }
        if(hookTimer < 0.2f)
        {
            // Hook on delay, can't shoot again
            GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayHookDelay();
            return;
        }

        // Start hook delay timer
        hookTimer = 0;

        GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayHookShoot();
        hook.SetActive(true);
        hookRigidbody.bodyType = RigidbodyType2D.Dynamic;
        hook.transform.position = transform.position;

        Vector2 hookTarget = crosshair.transform.position;
        if (hookTarget == (Vector2)transform.position)
        {
            // This means there is no crosshair, so shoot upwards
            hookTarget.y += 4.5f;
            if (spriteRenderer.flipX) // This can't be the horizontal movement because it should work when the dude is not moving
                hookTarget.x += 4.5f; // Loking to the right
            else
                hookTarget.x -= 4.5f;
        }

        Vector2 fromPlayerToHook = hookTarget - (Vector2)transform.position ;
        fromPlayerToHook.Normalize();

        // Shoot the hook
        hook.GetComponent<Rigidbody2D>().velocity = fromPlayerToHook * hookShootForce;
    }

    private void OnShoot(InputValue inputValue) 
    {
    }

    private void Move()
    {
        if (playerRigidbody.velocity.x < maxMoveSpeed && playerRigidbody.velocity.x > maxMoveSpeed * -1) 
        {
            if(horizontalMovement > 0)
            {
                spriteRenderer.flipX = true;
                playerRigidbody.velocity += Vector2.right * (moveSpeed * Time.deltaTime * horizontalMovement);
            } else if(horizontalMovement < 0)
            {
                spriteRenderer.flipX = false;
                playerRigidbody.velocity += Vector2.left * (moveSpeed * Time.deltaTime * (horizontalMovement * -1));
            }
        } 
    } 

    private void Animate()
    {
        // TODO: Make support for multiple skins
        if(horizontalMovement != 0)
        {
            GetComponent<Animator>().Play("FatsoRunning");
        }
        else
        {
            GetComponent<Animator>().Play("FatsoIdle");
        }
    }

    private bool IsHookAttached()
    {
        return hook.activeInHierarchy && hook.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Static;
    }
}
