using UnityEngine;

public class Recoil : MonoBehaviour
{
    private Gun_Controller gunController;
    private Vector3 currentRotation;
    private Vector3 targetRotation;


    [SerializeField] private float recoilSmoothness = 1f;
    [SerializeField] private float recoilSpeed = 5f;
    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;

    private void Start()
    {
        gunController = GetComponentInParent<Gun_Controller>();
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
