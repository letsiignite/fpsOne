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
        public int damageMultiplier = 0;        // 100 for head | 0 for legs and hand | 2 for cheast
        public bool isPlayer = false;
        public Vector3 damagePosition;
        public damageIndicator myDamageIndicator;
        private EnemyGun enemyGun;
        private bool isSniperHit = false;
        private float totalDamageRecived = 0;

        private void Start()
        {
            damageHandler = damageHandlerObject.GetComponent<IDamageHandler>();
            enemyGun = FindFirstObjectByType<EnemyGun>();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (isSniperHit && other.gameObject.GetComponent<SniperBullet>() != null)
            {
                isSniperHit = false;
                Debug.Log("  OnTriggerEnter totalDamageRecived = "+ totalDamageRecived);
                damageHandler.ProcessDamage(damageMultiplier, totalDamageRecived, out bool none);
                return;
            }
            damagePosition = other.transform.position;
            if (other.gameObject.GetComponent<Bullet>() != null)
            {
                damageHandler.ProcessDamage(damageMultiplier, other.gameObject.GetComponent<Bullet>().GetDamage(), out bool none);
            }
        }

        public void ReceiveRayHitDamage(float damage, Vector3 damagePosition, out bool isDead, bool isSniper = false)
        {
            //Debug.Log(  "  ReceiveRayHitDamage");
            if (isSniper)
            { 
                totalDamageRecived = damage;
                isSniperHit = true;
                isDead = true;
                return;
            }
            this.damagePosition = damagePosition;
            damageHandler.ProcessDamage(damageMultiplier, damage, out isDead);
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
            //Debug.Log(" Damage Indicator Enabled ");
            go.SetActive(true);
        }
        public void ReceiveGrenadeDamage(float damage, Vector3 pos)
        {
            //Debug.Log("  Grenade Damage on = "+gameObject.name);
            damageHandler.ProcessDamage(damageMultiplier, damage, out bool none); // damageMultiplier is handeled in grenade script.
            this.damagePosition = pos;
            if (isPlayer)
            {
                damageIndicatorEnable();
            }
        }
    } 
}
