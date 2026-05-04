using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyAssets : MonoBehaviour
{
    [SerializeField] private AudioClip[] onDeathAudioClips;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip onHit;
    [SerializeField] private AudioClip charge;
    public void PlayRandomAudioOnDeath()
    {
        if (onDeathAudioClips == null || onDeathAudioClips.Length == 0) return;

        int index = Random.Range(0, onDeathAudioClips.Length);
        audioSource.clip = onDeathAudioClips[index];
        audioSource.Play();
    }

    public void PlayAudioForAttackBehavior(bool isCharging)
    {
        if (isCharging)
        {
            audioSource.clip = charge;
        }
        else
        {
            audioSource.clip = onHit;
        }
            
        audioSource.Play();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
