using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSystem : MonoBehaviour
{
    //FIXME: Please build a better SoundSystem
    public void PlayHookHit()
    {
        transform.Find("HookHit").GetComponent<AudioSource>().Play();
    }
    public void PlayHookShoot()
    {
        transform.Find("HookShoot").GetComponent<AudioSource>().Play();
    }
    public void PlayHookRetract()
    {
        transform.Find("HookRetract").GetComponent<AudioSource>().Play();
    }
    public void PlayHookDelay()
    {
        transform.Find("HookDelay").GetComponent<AudioSource>().Play();
    }
    public void StopHookShoot()
    {
        var audio = transform.Find("HookShoot").GetComponent<AudioSource>();
        if(audio.isPlaying)
            StartCoroutine (FadeOut (audio, 0.2f));
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
    public void PlayElastic()
    {
        transform.Find("Elastic").GetComponent<AudioSource>().Play();
    }

 
 
    public IEnumerator FadeOut (AudioSource audioSource, float FadeTime) {
        float startVolume = audioSource.volume;
 
        while (audioSource.volume > 0) {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
 
            yield return null;
        }
 
        audioSource.Stop();
        audioSource.volume = startVolume;
    }
 
}
 
