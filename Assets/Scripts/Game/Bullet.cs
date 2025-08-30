using UnityEngine;

namespace Game
{


    public class Bullet : MonoBehaviour
    {
        [SerializeField]
        private float damage = 10;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public float GetDamage()
        {  return damage; }
    }
}
