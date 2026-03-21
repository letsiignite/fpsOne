using UnityEngine;

namespace Game
{
    public class SniperBullet : MonoBehaviour
    {
        [SerializeField]
        private Camera bulletCamera;
        private GameObject playerCam;
        [SerializeField]
        private GameObject bloodSplashParticals;
        [SerializeField]
        private GameObject bulletMesh;
        [SerializeField]
        private Rigidbody rb;
        private Transform parent;
        private Vector3 hitPosition;


        bool cameraActive = false;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            parent = transform.parent;
        }

        public void ActivateCamera(GameObject cam, Vector3 pos)
        {
            //Invoke("DisableGameobject", 2f);
            Time.timeScale = 0.7f;
            bulletCamera.gameObject.SetActive(true);
            cameraActive = true;
            playerCam = cam;
            hitPosition = pos;
            Debug.Log(" >>  bullet cam Active - "+ playerCam.name);
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.GetComponent<DamageReceiver>() == null)
                return;

            if (bulletCamera != null)
                bulletCamera.gameObject.SetActive(false);

            ContactPoint contact = collision.contacts[0];
            Vector3 hitPoint = contact.point;
            bloodSplashParticals.transform.position = hitPoint;

            bulletMesh.SetActive(false);
            //bloodSplashParticals.GetComponent<ParticleSystem>().Stop();
            bloodSplashParticals.SetActive(true);
            //bloodSplashParticals.GetComponent<ParticleSystem>().Play();
            Invoke("DisableGameobject", 1f);
            Debug.Log("++ >>  OnCollisionEnter");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<DamageReceiver>() == null)
                return;

            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            bloodSplashParticals.transform.position = bulletMesh.transform.position;
            bulletMesh.SetActive(false);
            //bloodSplashParticals.GetComponent<ParticleSystem>().Stop();
            bloodSplashParticals.SetActive(true);
            //bloodSplashParticals.GetComponent<ParticleSystem>().Play();
            Invoke("DisableGameobject", 1f);
            Debug.Log("++ >>  OnTriggerEnter for - "+other.name);
        }

        private void DisableGameobject()
        {
            if (!gameObject.activeInHierarchy)
                return;
            Time.timeScale = 1;
            gameObject.SetActive(false);
            playerCam.SetActive(true);
            transform.SetParent(parent);
            transform.localPosition = Vector3.zero; 
            Debug.Log(" >>  DisableGameobject ");
        }

        public void SetParent(Transform parent)
        { 
            this.parent = parent;
        }
    }
}