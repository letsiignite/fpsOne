using UnityEngine;
using UnityEngine.UI;

public class damageIndicator : MonoBehaviour
{
    public Vector3 DamageLocation;
    public Transform PlayerObject;
    public Transform DamageImagePivot;

    public CanvasGroup DamageImageCanvas;
    public float FadeStartTime, FadeDuration;
    float maxFadeTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxFadeTime = FadeDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if (FadeStartTime > 0)
            FadeStartTime -= Time.deltaTime;
        else
        {
            FadeDuration -= Time.deltaTime;
            DamageImageCanvas.alpha = FadeDuration / maxFadeTime;
            if (FadeDuration <= 0)
                Destroy(this.gameObject);
        }
        DamageLocation.y = PlayerObject.position.y;
        Vector3 Direction = (DamageLocation - PlayerObject.position).normalized;
        float angle = (Vector3.SignedAngle(Direction, PlayerObject.forward, Vector3.up));
        DamageImagePivot.localEulerAngles = new Vector3(0, 0, angle);
    }
}
