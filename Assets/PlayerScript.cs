using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public new Rigidbody2D rigidbody;
    public new SpriteRenderer spriteRenderer;
    public GameObject hook;
    public float sideVelocity = 10;
    public float jumpForce = 10;
    public float topSpeed = 20;
    public float hookShootPower = 20;
    public int debugFPS = 60;
    public int turnSpeed = 200;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Application.targetFrameRate = debugFPS;// REMOVE

        #region PlayerMovement
        if (Input.GetKeyDown(KeyCode.W) && rigidbody.velocity.y < 1 && rigidbody.velocity.y > -1)
        {
            rigidbody.velocity += Vector2.up * jumpForce; // Gravity is not affected by framerate
        }

        if (rigidbody.velocity.x < topSpeed && rigidbody.velocity.x > topSpeed * -1) 
        {

            if (Input.GetKey(KeyCode.A))
            {
                spriteRenderer.flipX = false;

                rigidbody.velocity += Vector2.left * (sideVelocity * Time.deltaTime);
            } else if (Input.GetKey(KeyCode.D))
            {
                spriteRenderer.flipX = true;
                rigidbody.velocity += Vector2.right * (sideVelocity * Time.deltaTime);
            }
        } else
            {
            }
        #endregion

        #region Hook
        if (Input.GetMouseButtonDown(1))
        {
            hook.SetActive(true);
            hook.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            hook.transform.position = transform.position;


            Vector2 hookTarget = Utils.GetMousePosition();

            Vector2 fromPlayerToHook = hookTarget - (Vector2)transform.position ;
            fromPlayerToHook.Normalize();

            hook.GetComponent<Rigidbody2D>().velocity = fromPlayerToHook * hookShootPower;

        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            hook.SetActive(false);
        }
        #endregion

    }
}
