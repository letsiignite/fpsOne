using Game;
using UnityEngine;

namespace Enemy
{
    public class EnemyGun : MonoBehaviour, IEnemyGun
    {
        [SerializeField]
        private Transform shootPoint;
        [SerializeField]
        private float range;
        [SerializeField]
        private float damage = 30;
        public void Shoot()
        {
            Debug.Log(" Shooting ");
          
            RaycastHit hit;
            if (Physics.Raycast(shootPoint.transform.position + Vector3.up, shootPoint.transform.forward, out hit, range))
            {
                Debug.Log(" Hit = "+hit.collider.name);
                if (hit.transform.GetComponent<DamageReceiver>())
                {
                    hit.transform.GetComponent<DamageReceiver>().ReceiveRayHitDamage(damage);

                }

            }
        }
        void OnDrawGizmos()
        {

            Gizmos.color = Color.white;
            Gizmos.DrawRay(shootPoint.position + Vector3.up, shootPoint.forward * range);
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