using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mission
{
    public class CountdownTimer : MonoBehaviour
    {
        [SerializeField] private float startTime = 60f; // seconds
        [SerializeField] private TMP_Text timerText; // UI Text

        [SerializeField]
        private GameObject[] objectsToHide;
        [SerializeField]
        private GameObject[] objectsToShow;

        private float currentTime;
        private bool isRunning = true;

        public void PauseTimer() => isRunning = false;
        public void ResumeTimer() => isRunning = true;

        void Start()
        {

        }

        private void OnEnable()
        {
            currentTime = startTime;
            UpdateUI();
        }

        void Update()
        {
            if (!isRunning) return;

            currentTime -= Time.deltaTime;

            if (currentTime <= 0f)
            {
                currentTime = 0f;
                isRunning = false;
                OnTimerEnd();
            }

            UpdateUI();
        }

        void UpdateUI()
        {
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);

            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        public void AddBonusTime(float seconds)
        {
            currentTime += seconds;
        }

        void OnTimerEnd()
        {
            Debug.Log("Timer Finished!");
            // TODO: Trigger mission fail / success / event
        }
    }
}