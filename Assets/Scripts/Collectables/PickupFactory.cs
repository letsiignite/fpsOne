using System.Collections.Generic;
using UnityEngine;
namespace Collectables
{
    public static class PickupFactory
    {
        private static Dictionary<CollectableType, IPickupHandler> handlers;

        static PickupFactory()
        {
            handlers = new Dictionary<CollectableType, IPickupHandler>()
        {
            { CollectableType.Health, new HealthPickupHandler() },
            { CollectableType.Ammo, new AmmoPickupHandler() },
            { CollectableType.Grenade, new GrenadePickupHandler() },
            { CollectableType.Intel, new IntelPickupHandler() }
        };

        }

        public static IPickupHandler GetHandler(CollectableType type)
        {
            return handlers[type];
        }
    }
}