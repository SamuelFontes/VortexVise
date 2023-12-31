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
            rigidbody.bodyType = RigidbodyType2D.Static;
            exploded = true;
            return;
        }
        // TODO: Apply damage to things here

    }
}
