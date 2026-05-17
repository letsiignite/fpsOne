using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField]
    private Transform target;

    [Header("Follow Settings")]
    [SerializeField]
    private Vector3 offset;
    [SerializeField]
    private float followSmoothness = 5f;
    [SerializeField]
    private float rotationSmoothness = 5f;

    private Vector3 velocity;

    private void LateUpdate()
    {
        if (target == null)
            return;

        // Desired camera position
        Vector3 desiredPosition = target.position + target.TransformDirection(offset);

        // Smooth position movement
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            1f / followSmoothness
        );

        // Smooth rotation
        //Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);

        //transform.rotation = Quaternion.Slerp(
        //    transform.rotation,
        //    targetRotation,
        //    rotationSmoothness * Time.deltaTime
        //);
    }
}