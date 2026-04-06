using UnityEngine;

public class BulletFollowCam : MonoBehaviour
{
    public Transform target;          
    public float rotationSpeed = 60f;
    private float starttime = 0;
    private float duration = 1;
    private Vector3 startPos= Vector3.zero;

    private void Start()
    {
        startPos = transform.position;
    }
    private void OnEnable()
    {
        starttime = Time.time;
    }

    private void OnDisable()
    {
        transform.position = startPos;
    }

    void Update()
    {
        if (target == null || !gameObject.activeInHierarchy || !target.gameObject.activeInHierarchy) return;

        if ((starttime + duration) < Time.time)
            return;
        
        transform.RotateAround(target.position, Vector3.up, rotationSpeed * Time.deltaTime);
        transform.LookAt(target);
    }
}
