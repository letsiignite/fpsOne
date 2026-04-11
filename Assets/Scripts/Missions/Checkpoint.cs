using Enemy;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mission
{
    [RequireComponent(typeof(BoxCollider))]
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField]
        private CheckpointSystem checkpointSystem;
        [SerializeField]
        private AudioClip audioClip;
        [SerializeField]
        private string checkpointInfo;
        [SerializeField]
        private GameObject nextCheckpoint;
        [SerializeField]
        private GameObject objectiveMarker;

      [SerializeField]
        private List<GameObject> enemySoldiers;
        [SerializeField]
        private Objective checkpointObj;
        [SerializeField]
        private PopupHandler popupHandler;


        private bool checkpointInfoProvided = false;

        public AudioClip GetAudioClip()
        { 
            return audioClip;
        }

        public string GetCheckpointInfo()
        {
            return checkpointInfo;
        }

        private void OnEnable()
        {
            
           
        }

        private void HideObjects()
        { 
            objectiveMarker?.SetActive(false);
        }

        private void Start()
        {
            Debug.Log(" START "+gameObject.name);
            checkpointSystem = GameObject.FindAnyObjectByType<CheckpointSystem>();
            checkpointObj.init(this, popupHandler);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Debug.Log(" Show Objective");
                objectiveMarker?.SetActive(true);
            }
            if (Input.GetKeyUp(KeyCode.Tab))
            {
                Debug.Log(" Hide Objective");
                objectiveMarker?.SetActive(false);
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Player" && !checkpointInfoProvided)
            {
                checkpointSystem.ProvideCheckpointInfo(this);
                checkpointInfoProvided = true;
                Invoke("HideObjects", 4f);
                //this.gameObject.SetActive(false);
            }
        }

        private void Awake()
        {
            GetComponent<BoxCollider>().isTrigger = true;
            checkpointSystem = GameObject.FindFirstObjectByType<CheckpointSystem>();
        }

        public void Respawn()
        {
            foreach (var a in enemySoldiers)
            {
                a.GetComponent<IEnemySolder>().Reset();
            }

            this.gameObject.SetActive(true);
        }

        public void HandleObjectiveCompleted(string objectiveText, AudioClip clip, float delay)
        {
            StartCoroutine(ObjectiveCompleted(objectiveText, clip, delay)); // ✅ runs on Checkpoint
        }

        public IEnumerator ObjectiveCompleted(string objectiveText, AudioClip clip, float delay)
        {
            yield return new WaitForSeconds(delay);
            Debug.Log(" CP Objective Completed");
            checkpointSystem.OnCompletingObjective(objectiveText, clip);
            nextCheckpoint.SetActive(true);
        }

        
    }
}