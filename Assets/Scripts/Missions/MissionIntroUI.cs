using System.Collections;
using TMPro;
using UnityEngine;


namespace Mission
{

    [System.Serializable]
    public class IntroLine
    {
        public RectTransform rect;
        public TextMeshProUGUI text;

        [HideInInspector] public Vector2 onScreenPos;
        [HideInInspector] public Vector2 offScreenPos;
    }

    public class MissionIntroUI : MonoBehaviour
    {
        public IntroLine[] lines;

        [Header("Timing")]
        public float slideDuration = 0.8f;
        public float delayBetweenLines = 0.4f;
        public float stayDuration = 5f;
        public float shrinkDuration = 0.4f;
        public float shrinkDelay = 0.15f;
        public float initialDelay = 2f;
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private AudioClip textSlideAudioClip;

        void Awake()
        {
            foreach (var line in lines)
            {
                line.onScreenPos = line.rect.anchoredPosition;
                line.offScreenPos = line.onScreenPos + new Vector2(-400f, 0f);

                line.rect.anchoredPosition = line.offScreenPos;
                line.rect.localScale = Vector3.one;
            }
        }

        public void ShowMission(MissionData data)
        {
            foreach (var line in lines)
            {
                line.rect.anchoredPosition = line.offScreenPos;
                line.rect.localScale = Vector3.one;
                line.rect.gameObject.SetActive(true);
                line.rect.localScale = Vector3.one;
            }

            // Assign text (order matters)
            lines[0].text.text = data.missionName;
            lines[1].text.text = data.commandoName;
            lines[2].text.text = data.time;
            lines[3].text.text = data.location;

            StartCoroutine(PlaySequence());
        }

        IEnumerator PlaySequence()
        {
            yield return new WaitForSeconds(initialDelay);
            // --- SLIDE IN (staggered) ---
            for (int i = 0; i < lines.Length; i++)
            {
                StartCoroutine(SlideIn(lines[i]));
                yield return new WaitForSeconds(delayBetweenLines);
            }

            // Wait until last finishes
            yield return new WaitForSeconds(slideDuration);

            // --- STAY ---
            yield return new WaitForSeconds(stayDuration);

            // --- SHRINK OUT (staggered) ---
            for (int i = 0; i < lines.Length; i++)
            {
                StartCoroutine(ShrinkOut(lines[i]));
                yield return new WaitForSeconds(shrinkDelay);
            }
        }

        IEnumerator SlideIn(IntroLine line)
        {
            float t = 0;
            audioSource.Stop();
            audioSource.Play();
            while (t < slideDuration)
            {
                t += Time.deltaTime;
                float lerp = EaseOut(t / slideDuration);
                line.rect.anchoredPosition = Vector2.Lerp(line.offScreenPos, line.onScreenPos, lerp);
                yield return null;
            }
        }

        IEnumerator ShrinkOut(IntroLine line)
        {
            float t = 0;
            while (t < shrinkDuration)
            {
                t += Time.deltaTime;
                float lerp = EaseIn(t / shrinkDuration);
                line.rect.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, lerp);
                yield return null;
            }

            line.rect.gameObject.SetActive(false);
        }

        float EaseOut(float x) => 1 - Mathf.Pow(1 - x, 3);
        float EaseIn(float x) => x * x;
    }
}