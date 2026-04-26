using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class AudioFadeOut : MonoBehaviour
{
    public float fadeDuration = 10f; // last 10 seconds

    private AudioSource audioSource;
    private float initialVolume;
    private bool isFading = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        initialVolume = audioSource.volume;
    }

    void Update()
    {
        if (audioSource.clip == null || !audioSource.isPlaying)
            return;

        float timeLeft = audioSource.clip.length - audioSource.time;

        // 🔥 Start fading when last X seconds remain
        if (!isFading && timeLeft <= fadeDuration)
        {
            isFading = true;
        }

        // 🔥 Perform fade
        if (isFading)
        {
            float t = timeLeft / fadeDuration; // 1 → 0
            audioSource.volume = initialVolume * t;

            if (timeLeft <= 0f)
            {
                audioSource.volume = 0f;
                isFading = false;
            }
        }
    }
}