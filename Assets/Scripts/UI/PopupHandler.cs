using Collectables;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class PopupHandler : MonoBehaviour
{
    public static PopupHandler Instance;

    public GameObject popupUI;
    public TMP_Text itemNameText;
    public Image icon;

    private CollectableItem currentItem;
    private GameObject currentPlayer;
    [SerializeField]
    private AudioSource audioSource;

   private void Awake()
    {
        Instance = this;
        popupUI.SetActive(false);
    }

    public void Show(CollectableItem item, GameObject player)
    {
        currentItem = item;
        currentPlayer = player;

        itemNameText.text = item.GetDisplayName();
        icon.sprite = item.GetIcon();

        popupUI.SetActive(true);
    }

    public void ShowObectiveCollectionPopup(string objectiveTitle, Sprite objectiveIcon)
    {
        itemNameText.text = objectiveTitle;
        icon.sprite = objectiveIcon;

        popupUI.SetActive(true);
    }

    public void Hide()
    {
        popupUI.SetActive(false);
        currentItem = null;
        currentPlayer = null;
    }

    private void Update()
    {
        if (currentItem != null && Input.GetKeyDown(KeyCode.F))
        {
            currentItem.Collect(currentPlayer);
            audioSource.Stop();
            audioSource.clip = currentItem.GetAudioClip();
            audioSource.Play();
            Hide();
        }
    }
}