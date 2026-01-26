using UnityEngine;
using UnityEngine.UI;

public class DamageArcIndicator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Image arcImage;

    [Header("Arc Settings")]
    [SerializeField] private float arcSizeDegrees = 45f;
    [SerializeField] private float fadeDuration = 0.8f;
    [SerializeField] private float maxAlpha = 1f;

    private float currentAlpha;
    private float fadeTimer;

    void Awake()
    {
        if (!playerCamera)
            playerCamera = Camera.main;

        arcImage.fillAmount = 0f;
        SetAlpha(0f);
    }

    void Update()
    {
        if (currentAlpha <= 0f)
            return;

        fadeTimer += Time.deltaTime;
        float t = fadeTimer / fadeDuration;

        SetAlpha(Mathf.Lerp(currentAlpha, 0f, t));

        if (t >= 1f)
        {
            arcImage.fillAmount = 0f;
            currentAlpha = 0f;
        }
    }

    /// <summary>
    /// Call when damage is received
    /// </summary>
    public void ShowDamage(Vector3 damageSourcePosition)
    {
        Vector3 direction = damageSourcePosition - playerCamera.transform.position;
        direction.y = 0f;
        direction.Normalize();

        float angle = Vector3.SignedAngle(
            playerCamera.transform.forward,
            direction,
            Vector3.up
        );

        // Rotate arc toward damage source
        arcImage.rectTransform.localRotation =
            Quaternion.Euler(0f, 0f, -angle);

        // Convert degrees to fill amount
        arcImage.fillAmount = arcSizeDegrees / 360f;

        currentAlpha = maxAlpha;
        fadeTimer = 0f;

        SetAlpha(currentAlpha);
    }

    private void SetAlpha(float alpha)
    {
        Color c = arcImage.color;
        c.a = alpha;
        arcImage.color = c;
    }
}
