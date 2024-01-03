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
    private bool isAttached = false;
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

        }
    }

    private void OnMove(InputValue inputValue)
    {
        offsetChanger = inputValue.Get<Vector2>().y * -1;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayHookHit();
        GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayElastic();
        rigidbody.bodyType = RigidbodyType2D.Static;
        isAttached = true;
        hookPullOffset = originalPullOffset;
        
    }

    private void RenderRope()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        Vector3[] positions = { transform.position, player.transform.position };
        lineRenderer.SetPositions(positions);
    }

}
