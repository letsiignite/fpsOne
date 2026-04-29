using UnityEngine;

public class EnemyAssets : MonoBehaviour
{
    [SerializeField] private AudioClip[] onDeathAudioClips;
    [SerializeField] private AudioSource audioSource;

    public void PlayRandomAudioOnDeath()
    {
        if (onDeathAudioClips == null || onDeathAudioClips.Length == 0) return;

        int index = Random.Range(0, onDeathAudioClips.Length);
        audioSource.clip = onDeathAudioClips[index];
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
