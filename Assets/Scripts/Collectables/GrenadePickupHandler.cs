using UnityEngine;
namespace Collectables
{
    public class GrenadePickupHandler : IPickupHandler
    {
        WeaponSelection weaponSelectionObject;
        public void Apply(GameObject player, int value)
        {
            Debug.Log("Grenades Added: " + value);
            if (weaponSelectionObject == null)
            {
                weaponSelectionObject = GameObject.FindFirstObjectByType<WeaponSelection>();
            }
            weaponSelectionObject.AddAmmoToAllGunsInHand(value);
        }
    }
}