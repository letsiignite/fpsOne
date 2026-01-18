using Enemy;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace mission
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
        private List<EnemyAI> enemySoldiers;

        private bool checkpointInfoProvided = false;

        public AudioClip GetAudioClip()
        { 
            return audioClip;
        }

        public string GetCheckpointInfo()
        {
            return checkpointInfo;
        }

        public List<EnemyAI> GetEnemyList()
        {
            return enemySoldiers;
        }
        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Player" && !checkpointInfoProvided)
            {
                checkpointSystem.ProvideCheckpointInfo(this);
                checkpointInfoProvided = true;
                this.gameObject.SetActive(false);
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
                a.gameObject.SetActive(true);
                a.Reset();
            }
        }

        private void Start()
        {
            checkpointSystem = GameObject.FindAnyObjectByType<CheckpointSystem>();
        }
    }
}