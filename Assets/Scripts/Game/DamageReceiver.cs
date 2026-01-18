using UnityEngine;

/// <summary>
/// A trigger attached to a specific body part like head or cheast that sets the damage for player or enemy
/// </summary>
/// 
namespace Game
{
    public class DamageReceiver : MonoBehaviour
    {
        public GameObject damageHandlerObject;
        public IDamageHandler damageHandler;
        public int damageMultiplyer = 0;        // 100 for head | 0 for legs and hand | 2 for cheast
        public bool isPlayer = false;
        public Vector3 damagePosition;
        public damageIndicator myDamageIndicator;

        private void Start()
        {
            damageHandler = damageHandlerObject.GetComponent<IDamageHandler>();
            damagePosition = Vector3.zero;
        }
        private void OnTriggerEnter(Collider other)
        {
            damagePosition = other.transform.position;
            if (other.gameObject.GetComponent<Bullet>() != null)
            {
                damageHandler.ProcessDamage(damageMultiplyer, other.gameObject.GetComponent<Bullet>().GetDamage());
            }
        }

        public void ReceiveRayHitDamage(float damage, Vector3 damagePosition)
        {
            //Debug.Log(  "  ReceiveRayHitDamage");
            damageHandler.ProcessDamage(damageMultiplyer,damage);
            if (isPlayer)
            {
                damageIndicatorEnable();
            }
        }

        void damageIndicatorEnable()
        {
            myDamageIndicator.DamageLocation = Vector3.zero;
            GameObject go = Instantiate(myDamageIndicator.gameObject, myDamageIndicator.transform.position, myDamageIndicator.transform.rotation, myDamageIndicator.transform.parent);
            Debug.Log(" Damage Indicator Enabled ");
            go.SetActive(true);
        }
        public void ReceiveGrenadeDamage(float damage)
        {
            //Debug.Log("  Grenade Damage on = "+gameObject.name);
            damageHandler.ProcessDamage(damageMultiplyer, damage); // damageMultiplyer is handeled in grenade script.
        }
    } 
}
