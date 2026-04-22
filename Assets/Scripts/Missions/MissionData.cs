using UnityEngine;

[CreateAssetMenu(fileName = "MissionData", menuName = "Game/Mission Data")]
public class MissionData : ScriptableObject
{
    public string missionName;
    public string commandoName;
    public string time;
    public string location;
}