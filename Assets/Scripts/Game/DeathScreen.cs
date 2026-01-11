using System.Collections;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    float fadeDuration = 1;
    public GameObject deathScreen;
    public Image deathScreenImage;
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
        deathScreen.SetActive(true);
        StartCoroutine(HideHitIndicatorImage());
    }

    public void HIdeDeathScreen()
    {
        deathScreen.SetActive(false);
    }

    IEnumerator HideHitIndicatorImage()
    {
        deathScreen.SetActive(true);

        UnityEngine.Color color = deathScreenImage.color;
        // Fade In
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalized = t / fadeDuration;
            color.a = Mathf.Lerp(0f, 1f, normalized);
            deathScreenImage.color = color;
            yield return null;
        }
        color.a = 1f;
        deathScreenImage.color = color;

    }
}
