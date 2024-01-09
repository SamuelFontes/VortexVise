using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class DeathWaterScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayDeath();

            collision.gameObject.transform.position = new Vector3(0, 0);
            //if (collision.gameObject.layer == 11)
                //Utils.playerOneScore++;
            //else
                //Utils.playerTwoScore++;
        }

    }
}
