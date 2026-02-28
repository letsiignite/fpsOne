using Enemy;
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
        private EnemyGun enemyGun;

        private void Start()
        {
            damageHandler = damageHandlerObject.GetComponent<IDamageHandler>();
            enemyGun = FindFirstObjectByType<EnemyGun>();
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
            this.damagePosition = damagePosition;
            damageHandler.ProcessDamage(damageMultiplyer,damage);
            if (isPlayer)
            {
                damageIndicatorEnable();
            }
        }

        void damageIndicatorEnable()
        {
            myDamageIndicator.DamageLocation = damagePosition;
            myDamageIndicator.EnableArrow();
            GameObject go = myDamageIndicator.transform.GetChild(0).gameObject;
            Debug.Log(" Damage Indicator Enabled ");
            go.SetActive(true);
        }
        public void ReceiveGrenadeDamage(float damage, Vector3 pos)
        {
            //Debug.Log("  Grenade Damage on = "+gameObject.name);
            damageHandler.ProcessDamage(damageMultiplyer, damage); // damageMultiplyer is handeled in grenade script.
            this.damagePosition = pos;
            if (isPlayer)
            {
                damageIndicatorEnable();
            }
        }
    } 
}
