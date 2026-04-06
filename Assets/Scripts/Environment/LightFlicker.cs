using UnityEngine;
using System.Collections;

public class LightFlicker : MonoBehaviour
{
    [Header("Light Reference")]
    [SerializeField] private Light targetLight;

    [Header("Intensity Settings")]
    [SerializeField] private float minIntensity = 0.5f;
    [SerializeField] private float maxIntensity = 3f;
    [SerializeField] private float flickerSpeed = 0.05f;

    [Header("Flicker Behaviour")]
    [SerializeField][Range(0f, 1f)] private float flickerChance = 0.5f;
    [SerializeField] private bool smoothFlicker = false;

    [Header("Smooth Flicker Settings")]
    [SerializeField] private float smoothSpeed = 10f;

    private float originalIntensity;
    private Coroutine flickerCoroutine;

    private void Start()
    {
        if (targetLight == null)
            targetLight = GetComponent<Light>();

        originalIntensity = targetLight.intensity;

        StartFlicker();
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            float targetIntensity;

            if (Random.value < flickerChance)
                targetIntensity = Random.Range(minIntensity, maxIntensity);
            else
                targetIntensity = originalIntensity;

            if (smoothFlicker)
            {
                float elapsed = 0f;
                float startIntensity = targetLight.intensity;

                while (elapsed < flickerSpeed)
                {
                    elapsed += Time.deltaTime;
                    targetLight.intensity = Mathf.Lerp(
                        startIntensity,
                        targetIntensity,
                        elapsed * smoothSpeed
                    );
                    yield return null;
                }
            }
            else
            {
                targetLight.intensity = targetIntensity;
                yield return new WaitForSeconds(flickerSpeed);
            }
        }
    }

    public void StartFlicker()
    {
        if (flickerCoroutine != null)
            StopCoroutine(flickerCoroutine);

        flickerCoroutine = StartCoroutine(FlickerRoutine());
    }

    public void StopFlicker()
    {
        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
            flickerCoroutine = null;
        }

        targetLight.intensity = originalIntensity;
    }
}