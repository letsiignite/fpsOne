using UnityEngine;
using UnityEngine.UI;

public class damageIndicator : MonoBehaviour
{
    public Vector3 DamageLocation;
    public Transform PlayerObject;
    public Transform DamageImagePivot;
    public Image arrowImage;

    public CanvasGroup DamageImageCanvas;
    public float  FadeDuration;
    float maxFadeTime;
    public GameObject HitIndicatorObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxFadeTime = FadeDuration;
    }

    public void EnableArrow()
    {
        DamageImageCanvas.alpha = 1.0f;
        FadeDuration = maxFadeTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (!transform.GetChild(0).gameObject.activeInHierarchy)
            return;

        FadeDuration -= Time.deltaTime;
        DamageImageCanvas.alpha = FadeDuration / maxFadeTime;
        
        if (FadeDuration <= 0)
            transform.GetChild(0).gameObject.SetActive(false);

        DamageLocation.y = 0;
        Vector3 flatForwardForPlayer = PlayerObject.forward;
        flatForwardForPlayer.y = 0;
        flatForwardForPlayer.Normalize();

        Vector3 flatForward =
        Quaternion.Euler(0, PlayerObject.eulerAngles.y, 0) * Vector3.forward;

        Vector3 dir = (DamageLocation - PlayerObject.position).normalized;
       
        dir.y = 0;
        float angle = (Vector3.SignedAngle(flatForward, dir,  Vector3.up));

        /*Alternative implementation
         * if(Vector3.Distance(DamageLocation, PlayerObject.position) > 3)
        angle -= 180;*/
        //DamageImagePivot.localEulerAngles = new Vector3(0, 0, angle);

        DamageImagePivot.localRotation = Quaternion.Euler(0, 0, -angle);
    }
}
