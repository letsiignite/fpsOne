using UnityEngine;

namespace mission
{
    [CreateAssetMenu(fileName = "MissionIntroData", menuName = "Scriptable Objects/MissionIntroData")]
    public class MissionIntroData : ScriptableObject
    {
        public AudioClip[] nerrationClips;
        public GameObject animationObject;
        public AudioClip bgMusic;
        public string[] nerrationText;
    }
}