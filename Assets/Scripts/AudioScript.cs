using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour
{
    public void PlayHookHit()
    {

        transform.Find("HookHit").GetComponent<AudioSource>().Play();
    }
    public void PlayHookShoot()
    {
        transform.Find("HookShoot").GetComponent<AudioSource>().Play();
    }
    public void PlayRocketFire()
    {
        transform.Find("RocketFire").GetComponent<AudioSource>().Play();

    }
    public void PlayRocketHit()
    {
        transform.Find("RocketHit").GetComponent<AudioSource>().Play();
    }
    public void PlayJump()
    {
        transform.Find("Jump").GetComponent<AudioSource>().Play();
    }
    public void PlayDeath()
    {
        transform.Find("Death").GetComponent<AudioSource>().Play();
    }
}
