using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Video;


namespace Mission
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
        private CheckpointSystem checkpointSystem;
       
        [SerializeField]
        private MenuManager menuManager;
        [SerializeField] 
        private VideoPlayer videoPlayer;
        [SerializeField]
        private GameObject missionIntroImage;
       [SerializeField]
        private GameObject[] objectsToHideForIntro;
       public float RESPAWN_DELAY = 0;  
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //Invoke("StartMission", 0.3f); 
           StartCoroutine(StartMission());
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                menuManager.DisplayPauseMenu();
            }
        }

        public IEnumerator StartMission()
        {
            missionIntroImage.SetActive(true);
            yield return null;
            //yield return new WaitForSeconds (0.5f);
            GameManager.Instance.player.transform.position = PlayerSpawnPosForMission[missionIndex].transform.position;
            playerController = GameManager.Instance.player.GetComponent<PlayerController>();
            playerController.DisableMovement();
            bgAudioSource.Stop();
            //bgAudioSource.clip = introData[missionIndex].audioClip;
            //bgAudioSource.Play();
          
            GameManager.Instance.SetGameState(GameState.MissionIntro);
            DialogText.text = "...";
            //foreach (GameObject obj in objectsToHideForIntro)
            //{ 
            //    obj.SetActive(false);
            //}
            bgAudioSource.Stop();
            GameManager.Instance.SetGameState(GameState.Running);
            playerController.EnableMovement();
            missionIntroImage.SetActive(false);
            //StartCoroutine(HandleNerration());
        }

        IEnumerator HandleNerration()
        {
            Debug.Log("HandleNerration - "+Time.time);
           
            videoPlayer.clip = introData[missionIndex].nerrationVideoClip;
            videoPlayer.Prepare();
            yield return new WaitUntil(() => videoPlayer.isPrepared);
            Debug.Log("Prepare - " + Time.time);
            videoPlayer.Play();
            DialogText.text = introData[missionIndex].nerrationText;

            yield return new WaitWhile(() => videoPlayer.isPlaying);
            Debug.Log("Play time - " + Time.time);
            videoPlayer.Stop();
            videoPlayer.gameObject.SetActive(false);

            GameManager.Instance.SetGameState(GameState.Running);
            DialogText.text = "...";
            bgAudioSource.Stop();
            foreach (GameObject obj in objectsToHideForIntro)
            {
                obj.SetActive(true);
            }
            missionIntroImage.SetActive(false);
            playerController.EnableMovement();
        }

        public void EndMission()
        { 
        
        }

        public void DisablePlayerMovement()
        {
            playerController.DisableMovement();
        }

        public void EnablePlayerMovements()
        {

            playerController.EnableMovement();

        }

        public void Respawn()
        {
            StartCoroutine(ProcessRespawn(RESPAWN_DELAY));
        }

        IEnumerator ProcessRespawn(float delay)
        {
            CharacterController cc = GameManager.Instance.player.GetComponent<CharacterController>();

            cc.enabled = false;

            GameManager.Instance.player.transform.position = checkpointSystem.GetLastCheckpointPos();
            Debug.Log(" Plr Pos = " + GameManager.Instance.player.transform.position + " | Chk Pos = " + checkpointSystem.GetLastCheckpointPos());
            cc.enabled = true;

            playerController.ResetMovement();
            yield return new WaitForSeconds(delay);
           
            checkpointSystem.Respawn();
            playerController.EnableMovement();
            menuManager.HideMissionMenus();
            GameManager.Instance.SetGameState(GameState.Running);
        }
    }
}