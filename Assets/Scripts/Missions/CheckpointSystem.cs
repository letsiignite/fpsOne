using TMPro;
using UnityEngine;

namespace Mission
{
    public class CheckpointSystem : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text checkpointDialogText;
        [SerializeField]
        private AudioSource voiceOverAudioSource;

        private Checkpoint lastCheckpoint;

        public void OnCompletingObjective(string objectiveText, AudioClip clip)
        {
            checkpointDialogText.text = objectiveText;
            voiceOverAudioSource.Stop();
            voiceOverAudioSource.clip = clip;
            voiceOverAudioSource.Play();

            Invoke("ResetCheckpointData", clip.length + 1);
        }
        public void ProvideCheckpointInfo(Checkpoint checkpoint)
        {
            checkpointDialogText.text = checkpoint.GetCheckpointInfo();
            voiceOverAudioSource.Stop();
            voiceOverAudioSource.clip = checkpoint.GetAudioClip();
            voiceOverAudioSource.Play();

            lastCheckpoint = checkpoint;    
            Invoke("ResetCheckpointData", lastCheckpoint.GetAudioClip().length + 1);
        }

        private void ResetCheckpointData()
        {
            checkpointDialogText.text = "";
        }

        public Vector3 GetLastCheckpointPos()
        {
            return lastCheckpoint.transform.position;
        }

        public void Respawn()
        { 
            lastCheckpoint.Respawn();
        }
    }
}