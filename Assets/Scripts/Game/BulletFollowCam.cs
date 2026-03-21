using UnityEngine;

public class BulletFollowCam : MonoBehaviour
{
    public Transform target;          
    public float rotationSpeed = 60f;
    private float starttime = 0;
    private float duration = 1;

    private void OnEnable()
    {
        starttime = Time.time;
    }

    void Update()
    {
        
        if (target == null || !gameObject.activeInHierarchy || !target.gameObject.activeInHierarchy) return;
        transform.RotateAround(target.position, Vector3.up, rotationSpeed * Time.deltaTime);
        transform.LookAt(target);
    }
}
