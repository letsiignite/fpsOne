/*using UnityEngine;

public class CameraShakeController : MonoBehaviour
{
	[SerializeField] private float shakePower;
    [SerializeField] private float shakeDuration;
    [SerializeField] private Transform mainCamera;
    [SerializeField] private float slowdownAmount;
	[HideInInspector]
    public bool canShake = false;

    private Vector3 startPosition;
    private float initialDuration;

    private void Start()
    {
	    mainCamera = Camera.main.transform;
	    startPosition = mainCamera.localPosition;
	    initialDuration = shakeDuration;
    }

    private void Update()
    {
        RotateCameraAndShake();       
    }

    private void RotateCameraAndShake()
    {
   
		
		 GameObject[] explosion = GameObject.FindGameObjectsWithTag("Explosion");
	     foreach(GameObject target in explosion) {
         float distance = Vector3.Distance(target.transform.position, transform.position);
         if(distance < 25) {
             canShake = true;
			 shakePower = (distance-26)*0.04f;
         }
     }

     if(canShake)
	{
		if(shakeDuration > 0)
		{
			mainCamera.localPosition = startPosition + Random.insideUnitSphere * (-shakePower);
			shakeDuration -= Time.deltaTime * slowdownAmount;
		}
		else
		{
			canShake = false;
			shakeDuration = initialDuration;
			mainCamera.localPosition = startPosition;
		}
	}
	}	
}
*/


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CameraShakeController : MonoBehaviour
{
    [SerializeField] private float shakePower = 0.5f;
    [SerializeField] private float duration = 10f;
    public bool canShake = false;

    void Update()
    {
        CameraShake();
    }
    IEnumerator Shake(float duration, float magnitude)
	{
        
        Vector3 originalPos = transform.localPosition;
		float elapsed = 0.0f;
		while (elapsed < duration)
		{
			float x = Random.Range(-1f, 1f) * magnitude;
			float y = Random.Range(-1f, 1f) * magnitude;
			transform.localPosition = new Vector3(x, y, originalPos.z);
			elapsed += Time.deltaTime;
			yield return null;
		}
		transform.localPosition = originalPos;
	}

    void CameraShake()
    {
        GameObject[] explosion = GameObject.FindGameObjectsWithTag("Explosion");
        float elapsed = 0.0f;
        foreach (GameObject target in explosion)
        {
            //float elapsed = 0.0f;
            float distance = Vector3.Distance(target.transform.position, transform.position);
            if (distance < 25 && elapsed < duration)
            {
                //float elapsed = 0.0f;
                canShake = true;
                shakePower = (26 - distance) * 0.04f;
                StartCoroutine(Shake(duration, shakePower));
                elapsed += Time.deltaTime;
                Debug.Log(elapsed);
                //while(elapsed < duration)
                {
                }
                //Destroy(target);
            }
        }
    }
}