using System.Collections;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    float fadeDuration = 1;
    public GameObject deathScreen;
    public Image deathScreenImage;
    public bool isDead = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isDead = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            StartCoroutine("DeathScreenLoader");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isDead = false;
            deathScreenImage.enabled = true;
        }
    }

    public IEnumerator DeathScreenLoader()
    {
        yield return new WaitForSeconds(2);
        deathScreen.SetActive(true);
    }
    public void DisplayDeathScreen()
    {
        deathScreen.SetActive(true);
        StartCoroutine(HideHitIndicatorImage());
    }
    public void Restart()
    {
        SceneManager.LoadScene(0);
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
