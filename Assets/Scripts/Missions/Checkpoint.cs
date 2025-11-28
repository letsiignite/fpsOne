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

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Player")
            {
                checkpointSystem.ProvideCheckpointInfo(audioClip, checkpointInfo);
            }
        }

        private void Awake()
        {
            GetComponent<BoxCollider>().isTrigger = true;
            checkpointSystem = GameObject.FindFirstObjectByType<CheckpointSystem>();
        }
    }
}