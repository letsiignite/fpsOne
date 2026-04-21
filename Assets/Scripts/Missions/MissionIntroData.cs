using UnityEngine;
using UnityEngine.Video;

namespace Mission
{
    [CreateAssetMenu(fileName = "MissionIntroData", menuName = "Scriptable Objects/MissionIntroData")]
    public class MissionIntroData : ScriptableObject
    {
        public VideoClip nerrationVideoClip;
        public string nerrationText;
    }
}