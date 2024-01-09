using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HookScript : MonoBehaviour
{
    public new Rigidbody2D rigidbody;
    public Player player;
    public Rigidbody2D playerRigidbody;
    public GameObject Impact;
    public GameObject ShootingEffect;

    public float hookForce = 10;
    public float hookPullOffset = 5;
    public float CameraShake = 0.5f;
    public float CameraShakeDuration = 0.2f;

    float originalPullOffset;
    float offsetChanger;
    Gamepad gamepad;

    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = player.GetComponent<Rigidbody2D>();
        originalPullOffset = hookPullOffset;
        gamepad = player.GetComponent<PlayerInput>().GetDevice<Gamepad>();
    }

    float gambiSoundTimer = 0; // TODO: PLEASE MOVE THIS TO SOUND SYSTEM, it should have an option to only play the sound after some time
    // Update is called once per frame
    void Update()
    {
        if (gambiSoundTimer > 1f)
            gambiSoundTimer = 0; 
        else if(gambiSoundTimer > 0)
            gambiSoundTimer += Time.deltaTime;

        RenderRope();
        if(rigidbody.bodyType == RigidbodyType2D.Static)
        {
            Vector2 fromPlayerToHook = (Vector2)transform.position - (Vector2)player.transform.position;
            fromPlayerToHook.Normalize();
            float distance = Vector2.Distance(transform.position, player.transform.position);

            if((hookPullOffset > originalPullOffset && offsetChanger < 0) || (hookPullOffset < originalPullOffset * 6 && offsetChanger > 0))
            {
                hookPullOffset += offsetChanger * Time.deltaTime * 10;

                if(gambiSoundTimer == 0) // TODO: PLEASE MOVE THIS TO SOUND SYSTEM, it should have an option to only play the sound after some time
                {
                    GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayElastic();// TODO: PLEASE MOVE THIS TO SOUND SYSTEM, it should have an option to only play the sound after some time
                    gambiSoundTimer += Time.deltaTime;
                }
            }




            if (distance > hookPullOffset)
            {
                playerRigidbody.velocity += fromPlayerToHook * hookForce * Time.deltaTime;
            }

        }
    }

    void OnMove(InputValue inputValue)
    {
        offsetChanger = inputValue.Get<Vector2>().y * -1;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // TODO: make smoke come out when contact, also make smoke when shooting, also make camera shake
        Utils.GamepadRumble(gamepad,0f,1f,0.2f);
        GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayHookHit();
        GameObject.FindWithTag("AudioSystem").GetComponent<AudioSystem>().PlayElastic();
        rigidbody.bodyType = RigidbodyType2D.Static;
        hookPullOffset = originalPullOffset;
        Instantiate(Impact, transform.position, transform.rotation);
        player.camera.GetComponent<PlayerCamera>().StartShake(CameraShakeDuration, CameraShake);
    }

    void RenderRope()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        Vector3[] positions = { transform.position, player.transform.position };
        lineRenderer.SetPositions(positions);
    }
    public void PlayShootAnimataion()
    {
        Instantiate(ShootingEffect, transform.position, transform.rotation);
    }

}
