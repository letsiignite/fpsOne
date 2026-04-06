using UnityEngine;
namespace Collectables
{
    public class IntelPickupHandler : IPickupHandler
    {
        public void Apply(GameObject player, int value)
        {
            Debug.Log("Intel Collected");
            // MissionManager hook here
        }
    }
}