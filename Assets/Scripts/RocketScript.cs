using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketScript : MonoBehaviour
{
    public float explosionDuration = 3f;
    public float explosionSize = 10;

    private float timer = 0f;
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

        if (exploded) 
        { 
            timer += Time.deltaTime;
            transform.localScale = transform.localScale + new Vector3(explosionSize * Time.deltaTime, explosionSize * Time.deltaTime);

            spriteRenderer.color -= new Color (0, 0, 0, 2 * Time.deltaTime);
            if(timer > explosionDuration)
            {
                Object.Destroy(this.gameObject);
            }
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!exploded)
        {
            GameObject.FindWithTag("AudioSystem").GetComponent<AudioScript>().PlayRocketHit();
            rigidbody.bodyType = RigidbodyType2D.Static;
            exploded = true;
            return;
        }
        if (collision.gameObject.tag == "Player")
        {
            GameObject.FindWithTag("AudioSystem").GetComponent<AudioScript>().PlayDeath();

            collision.gameObject.transform.position = new Vector3(0, 0);

            if (collision.gameObject.layer == 11)
                Utils.playerOneScore++;
            else
                Utils.playerTwoScore++;
        }
        // TODO: Apply damage to things here

    }
}
