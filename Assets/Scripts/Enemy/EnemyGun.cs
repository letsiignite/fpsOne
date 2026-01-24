using Game;
using System.Collections;
using System.Collections.Generic;
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

        private Vector3 dir;
        private float trailSpawnDelay = 0.3f;
        private int shootCount = 0;
        public void Shoot(Vector3 dir, int shootCount)
        {
            //Debug.Log(" Shooting ");
            this.dir = dir;
            RaycastHit hit;
            this.shootCount = shootCount;
            if (Physics.Raycast(shootPoint.transform.position, dir, out hit, range, layerMask))
            {
                //Debug.Log(" Hit = "+hit.collider.gameObject.name);
               
                if (hit.collider.gameObject.GetComponent<DamageReceiver>() != null)
                {
                    hit.collider.gameObject.GetComponent<DamageReceiver>().ReceiveRayHitDamage(damage);
                    //Debug.Log(" Calling - ReceiveRayHitDamage "+damage);
                }
                StartCoroutine(SpawnBulletTrail(hit));
            }
        }

        private IEnumerator SpawnBulletTrail(RaycastHit hit)
        {
            while (shootCount > 0)
            {
                yield return new WaitForSeconds(trailSpawnDelay);    
                GameObject obj = Instantiate(BulletTrailPrefab, shootPoint.transform.position, Quaternion.LookRotation(dir, Vector3.up));
                obj.GetComponent<BulletTrail>().Init(hit.point, bulletHitEffect);
                shootCount--;
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

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}