using UnityEngine;

namespace Collectables
{
    public interface IPickupHandler
    {
        void Apply(GameObject player, int value);
    }
}