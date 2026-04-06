//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;
//public class CameraShakeController : MonoBehaviour
//{
//    [SerializeField] private float shakePower = 0.5f;
//    [SerializeField] private float duration = 1f;
//    private Gun_Controller gunController;


//    void Start()
//    {
//        gunController = GetComponentInParent<Gun_Controller>();
//    }
//    void Update()
//    {
//        CameraShake();
//    }
//    public IEnumerator Shake(float duration, float magnitude)
//    {

//        Vector3 originalPos = transform.localPosition;
//        float elapsed = 0.0f;
//        while (elapsed < duration)
//        {
//            float x = Random.Range(-1f, 1f) * magnitude;
//            float y = Random.Range(-1f, 1f) * magnitude;
//            transform.localPosition = new Vector3(x, y, originalPos.z);
//            elapsed += Time.deltaTime;
//            yield return null;
//        }
//        transform.localPosition = originalPos;
//    }

//    void CameraShake()
//    {
//        if (!gunController.canShake) {return; }
//        float elapsed1 = 0.0f;
//        {
//            //float elapsed = 0.0f;
//            if ( elapsed1 < duration)
//            {
//                //float elapsed = 0.0f;
//                shakePower = (26) * 0.04f;
//                StartCoroutine(Shake(duration, shakePower));
//                elapsed1 += Time.deltaTime;
//                Debug.Log(elapsed1);
//                //while(elapsed < duration)
//                {
//                }
//                //Destroy(target);
//            }
//        }
//    }
//}

// CameraShake.cs (Attach to Main Camera)
using UnityEngine;
using System.Collections;

public class CameraShakeController : MonoBehaviour
{
    [SerializeField] public float duration = 0.5f; // How long the shake lasts
    [SerializeField] public float magnitude = 0.1f; // How strong the shake is (Unity units)
    [SerializeField] public float roughness = 10f; // Frequency of shake
    private Gun_Controller gunController;
    Vector3 originalPos;
    float elapsed;

    void Start()
    {
        originalPos = transform.localPosition;
        gunController = GetComponentInParent<Gun_Controller>();
    }

    public void TriggerShake(float shakePower)
    {
        magnitude = shakePower;
        if (elapsed == 0)
        { // Prevent overlapping shakes
            StartCoroutine(Shake());
        }
    }

    public IEnumerator Shake(float magnitude = 0.1f, float duration = 0.5f, float roughness = 10)
    {
        elapsed = duration; // Start timer
        while (elapsed > 0)
        {
            // Calculate random offset
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed -= Time.deltaTime * roughness; // Decrease timer with roughness
            yield return null; // Wait for next frame
        }
        transform.localPosition = originalPos; // Return to original position
        elapsed = 0; // Reset timer
    }
}

