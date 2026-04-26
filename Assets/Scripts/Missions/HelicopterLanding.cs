using UnityEngine;

public class HelicopterLanding : MonoBehaviour
{
    [Header("Landing Settings")]
    public float descendSpeed = 5f;
    public float groundCheckDistance = 10f;
    public LayerMask groundLayer;

    [Header("Smoothing")]
    public float smoothTime = 0.5f;

    public MenuManager menuManager;

    private float velocityY;
    private bool hasLanded = false;

    void OnEnable()
    {
        hasLanded = false;
        velocityY = 0f;
    }

    void Update()
    {
        if (hasLanded) return;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, groundLayer))
        {
            float targetY = hit.point.y;
            float currentY = transform.position.y;

            // Smooth descent
            float newY = Mathf.SmoothDamp(currentY, targetY, ref velocityY, smoothTime);

            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            // Touchdown check
            if (Mathf.Abs(newY - targetY) < 0.05f)
            {
                transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
                hasLanded = true;

                OnTouchdown();
            }
        }
        else
        {
            // If no ground detected, keep descending
            transform.position += Vector3.down * descendSpeed * Time.deltaTime;
        }
    }

    void OnTouchdown()
    {
        Debug.Log("Helicopter Landed");

        // Optional: disable physics or animation here
        // Example:
        // GetComponent<Rigidbody>().isKinematic = true;
        menuManager.DisplayMissionComplitionScreen();
    }
}