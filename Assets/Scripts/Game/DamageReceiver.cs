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

        private void Start()
        {
            damageHandler = damageHandlerObject.GetComponent<IDamageHandler>();
        }
        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.GetComponent<Bullet>() != null)
            {
                damageHandler.ProcessDamage(damageMultiplyer, other.gameObject.GetComponent<Bullet>().GetDamage());
              
            }
        }

        public void ReceiveRayHitDamage(float damage)
        {
            Debug.Log(  "  ReceiveRayHitDamage");
            damageHandler.ProcessDamage(damageMultiplyer,damage);
        }

        public void ReceiveGrenadeDamage(float damage)
        {
            Debug.Log("  Grenade Damage on = "+gameObject.name);
            damageHandler.ProcessDamage(damageMultiplyer, damage); // damageMultiplyer is handeled in grenade script.
        }
    } 
}
