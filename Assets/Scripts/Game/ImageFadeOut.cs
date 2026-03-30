using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageFadeOut : MonoBehaviour
{
    public Image image;
    public float duration = 0.3f;

    void OnEnable()
    {
        Color color = image.color;
        color.a = 1f;
        image.color = color;

        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float time = 0;
        Color color = image.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            color.a = 1f - (time / duration);
            image.color = color;
            yield return null;
        }

        color.a = 0f;
        image.color = color;
        gameObject.SetActive(false);
    }
}