using UnityEngine;

public class Recoil : MonoBehaviour
{
    private Gun_Controller gunController;
    private Vector3 currentRotation;
    private Vector3 targetRotation;
    private Crosshair crosshair;


    [SerializeField] private float recoilSmoothness = 1f;
    [SerializeField] private float recoilSpeed = 5f;
    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;

    public Vector2 GetRecoil()
    {
        Vector3 euler = transform.localRotation.eulerAngles;

        float x = euler.x;
        float z = euler.z;

        // Convert from 0–360 to -180 to 180
        if (x > 180) x -= 360;
        if (z > 180) z -= 360;

        return new Vector2(z, x); // screen X, Y
    }

    private void Start()
    {
        Gun_Controller[] childElements = GetComponentsInChildren<Gun_Controller>();
        foreach (var ele in childElements)
        {
            // Check if the child GameObject is active in the hierarchy
            if (ele.gameObject.activeInHierarchy)
            {
                gunController = ele.GetComponent<Gun_Controller>();
            }
        }

        targetRotation = Vector3.one;
        currentRotation = gunController.mainCamera.transform.rotation.eulerAngles;
    }
    // Update is called once per frame
    void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, recoilSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, recoilSmoothness * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }
    public void RecoilFire()
    {
        targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }
}
