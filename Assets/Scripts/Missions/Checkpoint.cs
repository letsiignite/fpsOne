using UnityEngine;

namespace mission
{
    [RequireComponent(typeof(BoxCollider))]
    public class Checkpoint : MonoBehaviour
    {
        private CheckpointSystem checkpointSystem;
        private AudioClip audioClip;
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
        }
    }
}