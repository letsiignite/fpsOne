using UnityEngine;


namespace Mission
{
    public class Objective : MonoBehaviour
    {
        private Checkpoint checkpoint;
        private PopupHandler popupHandler;
        [SerializeField]
        private string objectiveTitle;
        [SerializeField]
        private Sprite objectiveIcon;
        [SerializeField]
        private AudioClip audioClip;
        [SerializeField]
        private string infoText;
        [SerializeField]
        private GameObject[] objectsToHide;
        [SerializeField]
        private GameObject[] objectsToShow;
        [SerializeField]
        private float delayToPlayAudio = 0;

        private bool playerInRange = false;
        private ObjectiveMarker objectiveMarker;

        public void init(Checkpoint chk, PopupHandler popupHandler, ObjectiveMarker objectiveMarker)
        { 
            checkpoint = chk;
            this.popupHandler = popupHandler;
            this.objectiveMarker = objectiveMarker;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            { 
                popupHandler.ShowObectiveCollectionPopup(objectiveTitle, objectiveIcon);
                playerInRange = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                popupHandler.Hide();
                playerInRange = false;
            }
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F) && playerInRange)
            {
                objectiveMarker.ObjectiveMarkerHiddenState(true);
                checkpoint.HandleObjectiveCompleted(infoText, audioClip, delayToPlayAudio);
                
                popupHandler.Hide();
                playerInRange = false;
                foreach (GameObject obj in objectsToHide) 
                {
                    obj.SetActive(false);
                }

                foreach (GameObject obj in objectsToShow)
                {
                    obj.SetActive(true);
                }
            }
        }

        public void Reset()
        {
            foreach (GameObject obj in objectsToHide)
            {
                obj.SetActive(true);
            }

            foreach (GameObject obj in objectsToShow)
            {
                obj.SetActive(false);
            }
        }
    }
}