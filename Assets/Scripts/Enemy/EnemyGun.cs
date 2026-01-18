using Game;
using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class EnemyGun : MonoBehaviour, IEnemyGun
    {
        public LayerMask layerMask;
        [SerializeField]
        private Transform shootPoint;
        [SerializeField]
        private float range;
        [SerializeField]
        private float damage = 30;
        [SerializeField]
        private GameObject bulletHitEffect;
        [SerializeField]
        private GameObject BulletTrailPrefab;
        public GameObject weaponFlash;
        public GameObject weaponFlashLight;
        public float flashTime = 0.1f;

        private Vector3 dir;


        public void Shoot(Vector3 dir)
        {
            //Debug.Log(" Shooting ");
            this.dir = dir;
            RaycastHit hit;
            Debug.Log(" Flash duration START ");
            StartCoroutine(Flashing());
            if (Physics.Raycast(shootPoint.transform.position, dir, out hit, range, layerMask))
            {
                //Debug.Log(" Hit = "+hit.collider.gameObject.name);
               
                if (hit.collider.gameObject.GetComponent<DamageReceiver>() != null)
                {
                    hit.collider.gameObject.GetComponent<DamageReceiver>().ReceiveRayHitDamage(damage,shootPoint.transform.position);

                    Debug.Log(" Calling - ReceiveRayHitDamage "+damage);
                }
                GameObject obj = Instantiate(BulletTrailPrefab, shootPoint.transform.position, Quaternion.LookRotation(dir, Vector3.up));
                obj.GetComponent<BulletTrail>().Init(hit.point, bulletHitEffect);
            }
        }

        IEnumerator Flashing()
        {
            weaponFlash.SetActive(true);
            weaponFlashLight.SetActive(true);
            yield return new WaitForSeconds(flashTime);
            weaponFlash.SetActive(false);
            weaponFlashLight.SetActive(false);
            Debug.Log(" Flashing Coroutine END ");
        }
        void flashDuration(float time)
        {
            float elaspsed = 0f;
            while (true)
            {
                elaspsed += Time.deltaTime;
                if(elaspsed >= time)
                {
                    Debug.Log(" Flash duration END " + elaspsed);
                    weaponFlash.SetActive(false);
                    weaponFlashLight.SetActive(false);
                    break;
                }
            }
        }
        void OnDrawGizmos()
        {

            Gizmos.color = Color.black;
            Gizmos.DrawRay(shootPoint.position, dir * range);
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            weaponFlash.SetActive(false);
            weaponFlashLight.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}