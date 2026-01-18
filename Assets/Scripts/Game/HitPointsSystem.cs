using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField]
        private Sprite firstHit;
        [SerializeField]
        private Sprite SecondHit;
        [SerializeField]
        private Sprite ThirdHit;
        [SerializeField]
        private Image HitIndicatorImage;
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private DeathScreen deathScreen;
        [SerializeField]
        private GameManager gameManager;

        private const float firstHitDelay = 1;
        private const float SecondHitDelay = 1.5f;
        private const float ThirdHitDelay = 2;
        private const float fadeDuration = 0.5f;

        private void Start()
        {
            currentHealth = totalHealth;
            healthText.text = currentHealth.ToString();
        }
        public void ProcessDamage(float damageMultiplyer, float damage)
        {
            //Debug.Log(" Processing dam - "+damage);
            float totalDamage = damageMultiplyer * damage;
            if (armor > 0) 
            {
                armor = armor - totalDamage;
                totalDamage = (armor < 0)? (armor + totalDamage): 0 ;
            }
            currentHealth -= totalDamage;
            healthText.text = currentHealth.ToString();
            Sprite hitImage = null;
            float delayTimer = 0;
           
            audioSource.Play();
            if (currentHealth < 10)
            {
                delayTimer = ThirdHitDelay;
                hitImage = ThirdHit;
            }
            else if(currentHealth < 50)
            {
                delayTimer = SecondHitDelay;
                hitImage = SecondHit;
            }
            else if (currentHealth < 90)
            {
                delayTimer = SecondHitDelay;
                hitImage = firstHit;
            }
            /*if(currentHealth <= 0)
            {
                GameManager.Instance.SetGameState(GameState.PlayerKilled);
                deathScreen.DisplayDeathScreen();
                return;
            }*/
            StartCoroutine(HideHitIndicatorImage(delayTimer, hitImage));
            //Debug.Log(" Processing dam - " + damage+ " || currentHealth = " + currentHealth);
        }

        IEnumerator HideHitIndicatorImage(float delay, Sprite hitImage)
        {
            if (HitIndicatorImage == null)
            {
                Debug.LogWarning("No image assigned to fade!");
                yield break;
            }

            HitIndicatorImage.gameObject.SetActive(true);

            Color color = HitIndicatorImage.color;

            // Fade In
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                float normalized = t / fadeDuration;
                color.a = Mathf.Lerp(0f, 1f, normalized);
                HitIndicatorImage.color = color;
                yield return null;
            }
            color.a = 1f;
            HitIndicatorImage.color = color;

            // hold time
            yield return new WaitForSeconds(delay);

            // Fade Out
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                float normalized = t / fadeDuration;
                color.a = Mathf.Lerp(1f, 0f, normalized);
                HitIndicatorImage.color = color;
                yield return null;
            }
            color.a = 0f;
            HitIndicatorImage.color = color;

            HitIndicatorImage.gameObject.SetActive(false);
        }
    }
}