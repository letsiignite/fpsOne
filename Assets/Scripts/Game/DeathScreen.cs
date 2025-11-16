using System.Collections;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    float fadeDuration = 1;
    [SerializeField]
    Image DeathScreenImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayDeathScreen()
    {
        DeathScreenImage.gameObject.SetActive(true);
        StartCoroutine(HideHitIndicatorImage());
    }

    IEnumerator HideHitIndicatorImage()
    {
        DeathScreenImage.gameObject.SetActive(true);

        UnityEngine.Color color = DeathScreenImage.color;
        // Fade In
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalized = t / fadeDuration;
            color.a = Mathf.Lerp(0f, 1f, normalized);
            DeathScreenImage.color = color;
            yield return null;
        }
        color.a = 1f;
        DeathScreenImage.color = color;

    }
}
