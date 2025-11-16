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
        private MissionIntroData[] introData;
        private Transform[] PlayerSpawnPosForMission;
        private AudioSource bgAudioSource;
        private AudioSource voiceOverAudioSource;
        private TMP_Text DialogText;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void StartMission()
        { 
            GameManager.Instance.player.transform.position = PlayerSpawnPosForMission[missionIndex].transform.position;
            bgAudioSource.Stop();
            bgAudioSource.clip = introData[missionIndex].bgMusic;
            bgAudioSource.Play();
            StartCoroutine(HandleNerration());
        }

        IEnumerator HandleNerration()
        {
            while (nerrationIndex < introData[missionIndex].nerrationClips.Length)
            {
                voiceOverAudioSource.Stop();
                voiceOverAudioSource.clip = introData[missionIndex].nerrationClips[nerrationIndex];
                voiceOverAudioSource.Play();
                DialogText.text = introData[missionIndex].nerrationText[nerrationIndex++];
                yield return new WaitForSeconds(voiceOverAudioSource.clip.length);
            }
            GameManager.Instance.SetGameState(GameState.Running);
            yield return null;
        }

        public void EndMission()
        { 
        
        }
    }
}