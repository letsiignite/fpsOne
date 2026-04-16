using UnityEngine;
using System.Collections;

public class CameraShakeController : MonoBehaviour
{
    private Vector3 originalPos;
    private Coroutine currentShake;

    void Awake()
    {
        originalPos = transform.localPosition;
    }

    public void Shake(float duration = 0.1f, float intensity = 0.05f)
    {
        if (currentShake != null)
        {
            StopCoroutine(currentShake);
        }

        currentShake = StartCoroutine(ShakeRoutine(duration, intensity));
    }

    private IEnumerator ShakeRoutine(float duration, float intensity)
    {
        float timer = 0f;

        while (timer < duration)
        {
            float damper = 1f - (timer / duration); // smooth fade out

            Vector3 offset = Random.insideUnitSphere * intensity * damper;
            offset.z = 0f; // keep FPS camera stable in depth

            transform.localPosition = originalPos + offset;

            timer += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
        currentShake = null;
    }
}