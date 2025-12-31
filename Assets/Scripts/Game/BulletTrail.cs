using System.Collections;
using UnityEngine;

namespace Game
{
    public class BulletTrail : MonoBehaviour
    {
        private Vector3 targetPos = Vector3.zero;
        private Transform poolParent;
        private GameObject hitEffect = null;
        [SerializeField]
        private float speed = 1f;

        void Start()
        {
            poolParent = transform.parent;
        }

        public void Init(Vector3 pos, GameObject hitEffect)
        {
            transform.SetParent(null);
            this.hitEffect = hitEffect;
            targetPos = pos;
        }

        void Update()
        {
            if (Vector3.Distance(transform.position, targetPos) < 1)
            {
                transform.SetParent(poolParent);
                gameObject.SetActive(false);
            }
            else
            {
                transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
                if (hitEffect != null)
                {
                    hitEffect.transform.position = targetPos;
                    hitEffect.SetActive(true);
                }
            }
        }
    }
}