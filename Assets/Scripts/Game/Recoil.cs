using UnityEngine;

public enum RecoilType
{
    Accumulate,   // AR, SMG
    SingleKick    // Sniper, Shotgun
}

public class Recoil : MonoBehaviour
{
    [SerializeField] private Vector2 recoilAmount = new Vector2(0.3f, 0.15f);
    [SerializeField] private float snappiness = 6f;
    [SerializeField] private float returnSpeed = 2f;
    [SerializeField] private float returnDelay = 0.1f;
    private float lastFireTime;

    private Vector2 currentRotation;
    private Vector2 targetRotation;
    
    [SerializeField] private RecoilType recoilType;
    public void FireRecoil()
    {
        lastFireTime = Time.time;

        float x = Random.Range(-recoilAmount.x  , - recoilAmount.x * 0.8f);
        float y = Random.Range(-recoilAmount.y, recoilAmount.y);
        Debug.Log(" X = "+x+" , Y = "+y);
        Vector2 recoil = new Vector2(x, y);

        if (recoilType == RecoilType.Accumulate)
        {
            targetRotation += recoil;
        }
        else if (recoilType == RecoilType.SingleKick)
        {
            targetRotation = recoil;
        }

        // Clamp vertical recoil (prevents sky aiming)
        targetRotation.x = Mathf.Clamp(targetRotation.x, -5f, 0f);
    }

    public Vector2 GetRecoilRotation()
    {
        return currentRotation;
    }

    public void UpdateRecoil()
    {
        // Only start returning after delay (COD feel)
        if (Time.time > lastFireTime + returnDelay)
        {
            targetRotation = Vector2.Lerp(targetRotation, Vector2.zero, returnSpeed * Time.deltaTime);
        }

        // Smooth follow (snappy movement)
        currentRotation = Vector2.Lerp(currentRotation, targetRotation, snappiness * Time.deltaTime);

        // Snap to zero when very close (prevents drifting forever)
        if (currentRotation.magnitude < 0.01f)
        {
            currentRotation = Vector2.zero;
        }
    }
}
