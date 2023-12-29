using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeldsScript : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position;
        //transform.position += (Vector3)(new Vector2(0, -50));

        
    }
}
