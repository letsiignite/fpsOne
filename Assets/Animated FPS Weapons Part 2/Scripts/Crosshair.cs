using System.Collections;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private bool drawCrosshair = true;
    [SerializeField] private Color crosshairColor = Color.white; //The crosshair color

    [SerializeField] private int width = 2; //Crosshair width
    [SerializeField] private int height = 10; //Crosshair height

    [SerializeField] private Recoil recoil;
    [SerializeField] private float recoilMultiplier = 500f;

    private Vector2 hitOffset = Vector2.zero;
    private Coroutine hitRoutine;
    private bool hitRoutineActive = false;

    [System.Serializable]
    public class spreading
    {
        public float spread = 20.0f; //Adjust this for a bigger or smaller crosshair
        public float maxSpread = 60.0f;
        public float minSpread = 20.0f;
        public float spreadPerSecond = 30.0f;
        public float decreasePerSecond = 25.0f;
    }

    public spreading spread;

    private Texture2D tex;
    private GUIStyle lineStyle;

    private void Start()
    {
        crosshairColor = Color.white;
        gameObject.SetActive(false);
        tex = new Texture2D(1, 1);
        SetColor(tex, crosshairColor); //Set color
        lineStyle = new GUIStyle();
        lineStyle.normal.background = tex;
    }

    private void Update()
    {
        if (Input.GetButton("Fire1") || Input.GetKey(KeyCode.W))
        {
            spread.spread += spread.spreadPerSecond * Time.deltaTime * 5;  //Incremente the spread
        }
        else
        {
            spread.spread -= spread.decreasePerSecond * Time.deltaTime * 5; //Decrement the spread        
        }

        spread.spread = Mathf.Clamp(spread.spread, spread.minSpread, spread.maxSpread);
    }

    private void OnGUI()
    {
        Vector2 centerPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        centerPoint += hitOffset;

        Vector2 recoilOffset = Vector2.zero;
        /*if (recoil != null)
        {
            recoilOffset = recoil.GetRecoil() * recoilMultiplier;
            //Debug.Log(" recoilOffset = " + recoilOffset);
        }*/

        centerPoint += recoilOffset;
        
        if (drawCrosshair)
        {
            GUI.Box(new Rect(centerPoint.x - width / 2, centerPoint.y - (height + spread.spread), width, height), "", lineStyle);
            GUI.Box(new Rect(centerPoint.x - width / 2, centerPoint.y + spread.spread, width, height), "", lineStyle);
            GUI.Box(new Rect(centerPoint.x + spread.spread, (centerPoint.y - width / 2), height, width), "", lineStyle);
            GUI.Box(new Rect(centerPoint.x - (height + spread.spread), (centerPoint.y - width / 2), height, width), "", lineStyle);
        }
    }

    private void SetColor(Texture2D myTexture, Color myColor)
    {
        for (int y = 0; y < myTexture.height; ++y)
        {
            for (int x = 0; x < myTexture.width; ++x)
            {
                myTexture.SetPixel(x, y, myColor);
            }
        }

        myTexture.Apply();
    }

    public void ApplyHitOffset(float intensity = 2f, float duration = 1f)
    {
        if (hitRoutineActive)
            return;

        hitRoutineActive = true;
        StartCoroutine(HitOffsetRoutine(intensity, duration));
    }

    private IEnumerator HitOffsetRoutine(float intensity, float duration)
    {
        // Random offset in range (-intensity, intensity)
        hitOffset = new Vector2(
            Random.Range(-intensity, intensity),
            Random.Range(-intensity, intensity)
        );
        Debug.Log(" ++ hitOffset = "+ hitOffset);
        Vector2 startOffset = hitOffset;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // Smooth return (ease out)
            hitOffset = Vector2.Lerp(startOffset, Vector2.zero, t);

            yield return null;
        }

        hitOffset = Vector2.zero;
        hitRoutineActive = false;
    }
}
