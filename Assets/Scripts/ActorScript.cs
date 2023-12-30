using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActorScript : MonoBehaviour
{
    public GameObject hook;
    public GameObject crosshair;
    public float jumpForce = 25;
    public float moveSpeed = 80;
    public float maxMoveSpeed = 20;
    public float hookShootForce = 100;

    private PlayerControls playerControls;
    private Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;
    private float horizontal = 0;
    // Start is called before the first frame update
    void Start()
    {
        playerControls = new PlayerControls();
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Animate();
    }

    private void OnJump()
    {
        if (rigidbody.velocity.y == 0 || hook.active)
        {
            hook.SetActive(false);
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
}
