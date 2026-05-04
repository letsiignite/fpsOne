using Mission;
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
        private AudioClip normalHit;
        [SerializeField]
        private AudioClip heavyHit;

       [SerializeField]
        private GameManager gameManager;
        [SerializeField]
        private MissionManager missionManager;
        [SerializeField]
        private DamageDirection damageDirection;
        [SerializeField]
        private DamageArcIndicator damageArcDirection;
        [SerializeField]
        private MenuManager menuManager;
        [SerializeField] 
        private Crosshair crosshair;
        [SerializeField]
        private CameraShakeController cameraShake;

        private bool cameraShakeCompleted = false;
        private bool godMode = false;

        private const float crosshairHitIntensity = 20;
        private const float cameraShakeForce = 0.1f;
        private const float firstHitDelay = 1;
        private const float SecondHitDelay = 1.5f;
        private const float ThirdHitDelay = 2;
        private const float fadeDuration = 0.5f;

        private void Start()
        {
            currentHealth = totalHealth;
            healthText.text = currentHealth.ToString();
            damageDirection = GetComponent<DamageDirection>();
            damageArcDirection = GetComponent<DamageArcIndicator>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                godMode = !godMode;
                healthText.text = "GOD";
            }
        }

        public void Heal(int amount)
        { 
            currentHealth = (currentHealth + amount > 100)? 100 : (currentHealth + amount);
            if(!godMode)
            healthText.text = currentHealth.ToString();
        }

        public void ProcessDamage(float damageMultiplyer, float damage, out bool isDead)
        {
            isDead = false;
           
            float totalDamage = damageMultiplyer * damage;

            Debug.Log(" Processing dam - " + damage+ " | totalDamage = "+ totalDamage);

            if (armor > 0) 
            {
                armor = armor - totalDamage;
                totalDamage = (armor < 0)? (armor + totalDamage): 0 ;
            }
            if (!godMode)
            {
                currentHealth -= totalDamage;
                healthText.text = currentHealth.ToString();
            }
                
            Sprite hitImage = null;
            float delayTimer = 0;

            if (currentHealth < 50 && crosshair != null && !cameraShakeCompleted)
            {
                cameraShakeCompleted = true;
                crosshair.ApplyHitOffset(crosshairHitIntensity, 1f);
               
            }

            if (totalDamage > 50)
            {
                Debug.Log(" ++ Sniped ++");
                audioSource.Stop();
                audioSource.clip = heavyHit;
                audioSource.Play();
                cameraShake.Shake(0.5f, cameraShakeForce * 2);
                GameManager.Instance.ApplySniperHitToPlayer();
            }
            else
            {
                audioSource.Stop();
                audioSource.pitch = Random.Range(0.8f, 1.2f);
                audioSource.clip = normalHit;
                audioSource.Play();
                cameraShake.Shake(0.5f, cameraShakeForce);
            }
            
           
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
            if(currentHealth <= 0)
            {
                GameManager.Instance.SetGameState(GameState.PlayerKilled);
                menuManager.DisplayDeathScreen();

                missionManager.DisablePlayerMovement();
                
                //missionManager.Respawn();
                //float delay = missionManager.RESPAWN_DELAY;
                //Invoke("HideDeathScreen", delay);
                return;
            }
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

        public void HandleLoadLastCheckpoint()
        {
            currentHealth = totalHealth;
            healthText.text = currentHealth.ToString();
        }
    }
}