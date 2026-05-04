using Collectables;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class PopupHandler : MonoBehaviour
{
    public static PopupHandler Instance;

    public GameObject popupUIForGun;
    public GameObject popupUIForCollectables;
    public TMP_Text itemNameText;
    public Image icon;
    public TMP_Text collectablesItemNameText;
    public Image collectablesIcon;

    private CollectableItem currentItem;
    private GameObject currentPlayer;
    [SerializeField]
    private AudioSource audioSource;

   private void Awake()
    {
        Instance = this;
        popupUIForCollectables.SetActive(false);
    }

    public void ShowCollectablesPopup(CollectableItem item, GameObject player)
    {
        currentItem = item;
        currentPlayer = player;

        collectablesItemNameText.text = item.GetDisplayName();
        collectablesIcon.sprite = item.GetIcon();

        popupUIForCollectables.SetActive(true);
    }

    public void ShowObectiveCollectionPopup(string objectiveTitle, Sprite objectiveIcon)
    {
        collectablesItemNameText.text = objectiveTitle;
        collectablesIcon.sprite = objectiveIcon;

        popupUIForCollectables.SetActive(true);
    }

    public void Hide()
    {
        popupUIForCollectables.SetActive(false);
        currentItem = null;
        currentPlayer = null;
    }

    private void Update()
    {
        if (
            GameManager.Instance.GetGameState() == GameState.Pause ||
            GameManager.Instance.GetGameState() == GameState.PlayerKilled
            )
        {
            popupUIForCollectables.SetActive(false);
        }
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