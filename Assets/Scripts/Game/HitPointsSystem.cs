using TMPro;
using UnityEngine;

namespace Game
{
    public class HitPointsSystem : MonoBehaviour, IDamageHandler
    {
        [SerializeField]
        private float totalHealth = 100;
        private float currentHealth = 0;
        private float armor = 0;

        [SerializeField]
        private TMP_Text healthText;
        private const string healthTextPrefix = "HP : ";

        private void Start()
        {
            currentHealth = totalHealth;
        }
        public void ProcessDamage(float damageMultiplyer, float damage)
        {
            Debug.Log(" Processing dam - "+damage);
            float totalDamage = damageMultiplyer * damage;
            if (armor > 0) 
            {
                armor = armor - totalDamage;
                totalDamage = (armor < 0)? (armor + totalDamage): 0 ;
            }
            currentHealth -= totalDamage;
            healthText.text = healthTextPrefix + currentHealth;

        }
    }
}