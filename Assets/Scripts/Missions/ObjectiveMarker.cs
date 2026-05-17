using UnityEngine;
using TMPro;
namespace Mission
{
    public class ObjectiveMarker : MonoBehaviour
    {
        [Header("References")]
        public Transform target;
        public Camera cam;

        [Header("UI")]
        public RectTransform markerUI;
        public RectTransform arrowUI;

        public TextMeshProUGUI distanceText;
        public CanvasGroup canvasGroup;

        [Header("Settings")]
        public Vector3 worldOffset = new Vector3(0, 2f, 0);

        public float edgeOffset = 80f;

        public float minScale = 0.6f;
        public float maxScale = 1f;
        public float maxDistance = 150f;

        public float fadeSpeed = 8f;
        private bool isHidden = false;
        private void Update()
        {
            if (target == null || cam == null || isHidden)
                return;

            Vector3 worldPos = target.position + worldOffset;

            Vector3 screenPos = cam.WorldToScreenPoint(worldPos);

            bool isBehind = screenPos.z < 0;

            if (isBehind)
            {
                screenPos *= -1;
            }

            bool isOffScreen =
                isBehind ||
                screenPos.x <= 0 ||
                screenPos.x >= Screen.width ||
                screenPos.y <= 0 ||
                screenPos.y >= Screen.height;

            if (isOffScreen)
            {
                ShowOffscreen(screenPos);
            }
            else
            {
                ShowOnscreen(screenPos);
            }

            // Distance text
            float distance = Vector3.Distance(
                cam.transform.position,
                target.position
            );

            distanceText.text = Mathf.RoundToInt(distance) + "m";

            // Dynamic scale
            float t = Mathf.Clamp01(distance / maxDistance);

            float scale = Mathf.Lerp(maxScale, minScale, t);

            markerUI.localScale = Vector3.one * scale;
        }

        void ShowOnscreen(Vector3 screenPos)
        {
            markerUI.position = Vector3.Lerp(
                markerUI.position,
                screenPos,
                10f * Time.deltaTime
            );

            // Keep marker upright
            markerUI.rotation = Quaternion.identity;

            // Reset arrow rotation
            arrowUI.localRotation = Quaternion.identity;

            // Hide arrow
            arrowUI.gameObject.SetActive(false);

            canvasGroup.alpha = Mathf.Lerp(
                canvasGroup.alpha,
                1f,
                fadeSpeed * Time.deltaTime
            );
        }

        void ShowOffscreen(Vector3 screenPos)
        {
            Vector3 screenCenter = new Vector3(
                Screen.width,
                Screen.height,
                0
            ) / 2f;

            Vector3 dir = (screenPos - screenCenter).normalized;

            float x = Mathf.Clamp(
                screenCenter.x + dir.x * (screenCenter.x - edgeOffset),
                edgeOffset,
                Screen.width - edgeOffset
            );

            float y = Mathf.Clamp(
                screenCenter.y + dir.y * (screenCenter.y - edgeOffset),
                edgeOffset,
                Screen.height - edgeOffset
            );

            // Smooth movement
            markerUI.position = Vector3.Lerp(
                markerUI.position,
                new Vector3(x, y, 0),
                10f * Time.deltaTime
            );

            // IMPORTANT:
            // Keep whole marker upright
            markerUI.rotation = Quaternion.identity;

            // Rotate ONLY the arrow
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

            arrowUI.localRotation = Quaternion.Euler(0, 0, angle);

            arrowUI.gameObject.SetActive(true);

            // Slight transparency
            canvasGroup.alpha = Mathf.Lerp(
                canvasGroup.alpha,
                0.85f,
                fadeSpeed * Time.deltaTime
            );
        }

        private void OnDisable()
        {
            Debug.Log("Marker deactivated");
        }

        public void SetTarget(Transform target)
        { 
            this.target = target;
        }

        public void ObjectiveMarkerHiddenState(bool state)
        {
            isHidden = state;
            gameObject.SetActive(!isHidden);
        }
    }
}