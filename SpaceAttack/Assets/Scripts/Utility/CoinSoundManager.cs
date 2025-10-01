using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSoundManager : MonoBehaviour
{
    public static CoinSoundManager Instance;

    [Header("효과음 클립")]
    public AudioClip playerHitClip;   
    public AudioClip groundHitClip;   

    private AudioSource audioSource;
    private float lastGroundSoundTime = 0f;
    public float groundSoundCooldown = 0.2f; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void PlayPlayerHit()
    {
        audioSource.PlayOneShot(playerHitClip);
    }

    public void PlayGroundHit()
    { 
        if (Time.time - lastGroundSoundTime > groundSoundCooldown)
        {
            audioSource.PlayOneShot(groundHitClip);
            lastGroundSoundTime = Time.time;
        }
    }
}
