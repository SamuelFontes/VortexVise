using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class DeadlyZoneBehaviour : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // TODO: Cause some damage on the entity that enters this zone, also teleports if necessary
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
