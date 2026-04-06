using UnityEngine;

namespace Game
{
    public class SniperGun : MonoBehaviour
    {
        [SerializeField]
        private GameObject playerCam;
        [SerializeField]
        private GameObject bulletPrefab;
        [SerializeField]
        private Transform firePoint;
        [SerializeField]
        private Transform bulletPool;

        private GameObject bullett;
        private Vector3 point;

        public float bulletForce = 1500f;

        private void Update()
        {
            
        }
        public void Shoot(Vector3 hitPoint)
        {
            GameObject bullet = null;
            foreach (Transform child in bulletPool)
            {
                if (!child.gameObject.activeSelf)
                {
                    bullet = child.gameObject;
                    bullet.transform.SetParent(null);
                    bullet.transform.position = firePoint.position;
                    bullet.transform.LookAt(hitPoint);
                    bullet.SetActive(true);

                    bullett = bullet;
                    point = hitPoint;
                    break;
                }
                Debug.Log(" ** Got bullet from pool");
            }

            if (bullet == null)
            {
                bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                bullet.GetComponent<SniperBullet>().SetParent(bulletPool);
                Debug.Log(" **  bullet created");
            }

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.isKinematic = false;  
            rb.AddForce(bullet.transform.forward * bulletForce);

            SniperBullet camFollow = bullet.GetComponent<SniperBullet>();
            camFollow.ActivateCamera(playerCam, hitPoint);
        }
    }
}