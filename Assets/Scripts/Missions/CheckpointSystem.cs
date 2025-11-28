using TMPro;
using UnityEngine;

namespace mission
{
    public class CheckpointSystem : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text checkpointDialogText;
        [SerializeField]
        private AudioSource voiceOverAudioSource;
       
        public void ProvideCheckpointInfo(AudioClip clip, string info)
        {
            checkpointDialogText.text = info;
            voiceOverAudioSource.Stop();
            voiceOverAudioSource.clip = clip;
            voiceOverAudioSource.Play();
        }
    }
}