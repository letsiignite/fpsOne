using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class DamageIndicator : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float startAlpha = 1f;
    [SerializeField]
    private AnimationCurve fadeCurve =
        AnimationCurve.EaseInOut(0, 1, 1, 0);

    private RectTransform rectTransform;
    private Coroutine fadeRoutine;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(float angle, float duration, Action<DamageIndicator> onComplete)
    {
        rectTransform.localRotation = Quaternion.Euler(0f, 0f, -angle);

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeOut(duration, onComplete));
    }

    IEnumerator FadeOut(float duration, Action<DamageIndicator> onComplete)
    {
        float t = 0f;
        Color color = image.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = t / duration;

            color.a = fadeCurve.Evaluate(normalized) * startAlpha;
            image.color = color;

            yield return null;
        }

        onComplete?.Invoke(this);
    }
}
