using UnityEngine;
namespace Collectables
{
    public enum CollectableType
    {
        Health,
        Ammo,
        Grenade,
        Intel
    }

    public class CollectableItem : MonoBehaviour
    {
        [SerializeField]
        private CollectableType type;
        [SerializeField]
        private int value = 20;

        [Header("UI")]
        [SerializeField]
        private string displayName;
        [SerializeField]
        private Sprite icon;

        [SerializeField]
        private AudioClip audioOnPickup;

        public Sprite GetIcon() => icon;

        public string GetDisplayName() => displayName;

        public AudioClip GetAudioClip() => audioOnPickup;
       private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PopupHandler.Instance.Show(this, other.gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PopupHandler.Instance.Hide();
            }
        }

        public void Collect(GameObject player)
        {
            IPickupHandler handler = PickupFactory.GetHandler(type);
            handler.Apply(player, value);

            Destroy(gameObject);
        }
    }
}