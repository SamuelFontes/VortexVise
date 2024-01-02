using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HookScript : MonoBehaviour
{
    public Rigidbody2D rigidbody;
    public GameObject player;
    public Rigidbody2D playerRigidbody;

    public float hookForce = 10;
    public float hookPullOffset = 5;
    public float hookAnimationTimer = 0;

    private float originalPullOffset;
    private float offsetChanger;
    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = player.GetComponent<Rigidbody2D>();
        originalPullOffset = hookPullOffset;
    }

    // Update is called once per frame
    void Update()
    {
        RenderRope();
        if(rigidbody.bodyType == RigidbodyType2D.Static)
        {

            Vector2 fromPlayerToHook = (Vector2)transform.position - (Vector2)player.transform.position;
            fromPlayerToHook.Normalize();
            float distance = Vector2.Distance(transform.position, player.transform.position);

            if((hookPullOffset > originalPullOffset && offsetChanger < 0) || (hookPullOffset < originalPullOffset * 3 && offsetChanger > 0))
                hookPullOffset += offsetChanger * Time.deltaTime * 10;

            if (distance > hookPullOffset)
            {
                playerRigidbody.velocity += fromPlayerToHook * hookForce * Time.deltaTime;
            }
            // TODO: else make some streching sound


        }
    }

    private void OnMove(InputValue inputValue)
    {
        offsetChanger = inputValue.Get<Vector2>().y * -1;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject.FindWithTag("AudioSystem").GetComponent<AudioScript>().PlayHookHit();
        rigidbody.bodyType = RigidbodyType2D.Static;
        GameObject.FindWithTag("AudioSystem").GetComponent<AudioScript>().PlayElastic();
        hookPullOffset = originalPullOffset;
        
    }

    private void RenderRope()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        //lineRenderer.positionCount = 3;
        // TODO: Create animation and wip sound when shooting and retracting when released
        Vector3[] positions = {transform.position, player.transform.position, new Vector2(0,0)};
        lineRenderer.SetPositions(positions);

    }

}
