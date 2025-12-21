using System.Collections;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;


namespace mission
{
    public class MissionManager : MonoBehaviour
    {
        private int missionIndex = 0;
        private int nerrationIndex = 0;
        public MissionIntroData[] introData;
        [SerializeField]
        private Transform[] PlayerSpawnPosForMission;
        [SerializeField]
        private AudioSource bgAudioSource;
        [SerializeField]
        private AudioSource voiceOverAudioSource;
        [SerializeField]
        private TMP_Text DialogText;
        [SerializeField]
        private PlayerController playerController;
        [SerializeField]
        private GameObject[] missionIntroObject;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            StartMission();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void StartMission()
        { 
            GameManager.Instance.player.transform.position = PlayerSpawnPosForMission[missionIndex].transform.position;
            playerController = GameManager.Instance.player.GetComponent<PlayerController>();
            playerController.DisableMovement();
            bgAudioSource.Stop();
            bgAudioSource.clip = introData[missionIndex].bgMusic;
            bgAudioSource.Play();
            missionIntroObject[missionIndex].SetActive(true);
            GameManager.Instance.SetGameState(GameState.MissionIntro);
            DialogText.text = "...";
            //bgAudioSource.Stop();
            GameManager.Instance.SetGameState(GameState.Running);
            playerController.EnableMovement();
            missionIntroObject[missionIndex].SetActive(false);
            //StartCoroutine(HandleNerration());
        }

        IEnumerator HandleNerration()
        {
            while (nerrationIndex < introData[missionIndex].nerrationClips.Length)
            {
                //Debug.Log(" Start Nerration - " + nerrationIndex);
                voiceOverAudioSource.Stop();
                voiceOverAudioSource.clip = introData[missionIndex].nerrationClips[nerrationIndex];
                voiceOverAudioSource.Play();
                DialogText.text = introData[missionIndex].nerrationText[nerrationIndex++];
                yield return new WaitForSeconds(voiceOverAudioSource.clip.length);
            }
            GameManager.Instance.SetGameState(GameState.Running);
            DialogText.text = "...";
            bgAudioSource.Stop();
            playerController.EnableMovement();
            missionIntroObject[missionIndex].SetActive(false);
            yield return null;
        }

        public void EndMission()
        { 
        
        }
    }
}