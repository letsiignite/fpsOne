using Game;
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

        private Vector3 dir;
        public void Shoot(Vector3 dir)
        {
            Debug.Log(" Shooting ");
            this.dir = dir;
            RaycastHit hit;
            if (Physics.Raycast(shootPoint.transform.position, dir, out hit, range, layerMask))
            {
                //Debug.Log(" Hit = "+hit.collider.gameObject.name);
               
                if (hit.collider.gameObject.GetComponent<DamageReceiver>() != null)
                {
                    hit.collider.gameObject.GetComponent<DamageReceiver>().ReceiveRayHitDamage(damage);
                    Debug.Log(" Calling - ReceiveRayHitDamage "+damage);
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

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}