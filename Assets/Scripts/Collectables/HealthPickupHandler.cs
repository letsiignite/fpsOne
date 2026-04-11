using Game;
using UnityEngine;
namespace Collectables
{
    public class HealthPickupHandler : IPickupHandler
    {
        private HitPointsSystem hitPointsSystem;
        public void Apply(GameObject player, int value)
        {
            Debug.Log(" -- HealthPickupHandler -- Apply");
            hitPointsSystem  = (hitPointsSystem == null)? GameObject.FindFirstObjectByType<HitPointsSystem>() : hitPointsSystem;
            hitPointsSystem.Heal(value);
        }
    }
}