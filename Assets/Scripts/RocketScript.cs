using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RocketScript : MonoBehaviour
{
    public float explosionDuration = 3f;
    public float explosionSize = 10;
    public Gamepad gamepad;
    public GameObject explosion;

    private float timer = 0f;
    private float rumbleTimer = 0f;
    private bool exploded = false;
    private Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        timer += Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
       rumbleTimer += Time.deltaTime;
       if(rumbleTimer > 0.5f ) 
        {
            //gamepad.SetMotorSpeeds(0, 0);
        }

        if (exploded) 
        { 
            timer += Time.deltaTime;
            transform.localScale = transform.localScale + new Vector3(explosionSize * Time.deltaTime, explosionSize * Time.deltaTime);

            spriteRenderer.color -= new Color (0, 0, 0, 1 * Time.deltaTime);
            if(timer > explosionDuration)
            {
                //gamepad.SetMotorSpeeds(0, 0);
                Object.Destroy(this.gameObject);
            }
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!exploded)
        {
            GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayRocketHit();
            rigidbody.bodyType = RigidbodyType2D.Static;
            exploded = true;
            //Gamepad.current.SetMotorSpeeds(0.123f, 0.234f);
            return;
        }
        if (collision.gameObject.tag == "Player")
        {

            GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayDeath();
            Instantiate(explosion, collision.gameObject.transform.position,collision.gameObject.transform.rotation);
            explosion.GetComponent<ParticleSystem>().Play();

            collision.gameObject.transform.position = new Vector3(0, 0);

            //if (collision.gameObject.layer == 11)
                //Utils.playerOneScore++;
            //else
                //Utils.playerTwoScore++;
        }
        // TODO: Apply damage to things here

    }
}
