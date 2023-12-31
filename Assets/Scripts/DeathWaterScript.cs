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
}
