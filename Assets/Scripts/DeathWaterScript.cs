using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class DeathWaterScript : MonoBehaviour
{
    public int numberOfWaterPoints = 18;
    private SpriteShapeController controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<SpriteShapeController>();
    }

    // Update is called once per frame
    void Update()
    {
        var waterPoints = controller.spline;
        var b = waterPoints;

        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameObject.FindWithTag("AudioSystem").GetComponent<AudioScript>().PlayDeath();

            collision.gameObject.transform.position = new Vector3(0, 0);
            if (collision.gameObject.layer == 11)
                Utils.playerOneScore++;
            else
                Utils.playerTwoScore++;
        }

    }
}
