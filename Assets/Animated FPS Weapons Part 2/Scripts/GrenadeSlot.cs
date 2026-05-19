using TMPro;
using UnityEngine;

public class GrenadeSlot : MonoBehaviour
{

    [SerializeField]  private int _grenadeQuantity;
    [SerializeField] private TMP_Text grenadeCount;

    public int grenadeQuantity
    {
        get => _grenadeQuantity;

        set
        {
            _grenadeQuantity = value;
            grenadeCount.text = _grenadeQuantity.ToString();
        }
    }
    [SerializeField] private GUIStyle mystyle;
	

    // Use this for initialization
    void Start()
	{
		mystyle.fontSize = 20;
		mystyle.normal.textColor = Color.white;
        grenadeCount.text = _grenadeQuantity.ToString();
    }
	private void OnGUI()
	{
		//GUI.Label(new Rect(45, Screen.height - 95, 100, 50), grenadeQuantity + "", mystyle);
	}
}
