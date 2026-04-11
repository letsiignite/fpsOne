using UnityEngine;
namespace Collectables
{
    public class AmmoPickupHandler : IPickupHandler
    {
        WeaponSelection weaponSelectionObject;
        public void Apply(GameObject player, int value)
        {
            Debug.Log(" -- Ammo Added: " + value);
            if (weaponSelectionObject == null)
            {
                weaponSelectionObject = GameObject.FindFirstObjectByType<WeaponSelection>();
            }
            weaponSelectionObject.AddAmmoToAllGunsInHand(value);
        }
    }
}