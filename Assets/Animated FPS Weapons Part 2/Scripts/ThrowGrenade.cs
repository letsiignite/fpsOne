using UnityEngine;

public class ThrowGrenade : MonoBehaviour {
	
	[SerializeField] private GameObject grenade;
	[SerializeField] private Transform startPoint;
	[SerializeField] private float throwForce = 15f;

	private float extraForceMultiplier = 0.5f;

    private void OnEnable()
    {
        Debug.Log(" Grenade OnEnable");
       
    }

    public void HandleThrowGrenade()
    {
        GameObject gren = Instantiate(grenade, startPoint.position, startPoint.rotation) as GameObject;
        float extraForce = (startPoint.rotation.x > 0) ?
                            (startPoint.rotation.x * extraForceMultiplier) :
                            (startPoint.rotation.x * extraForceMultiplier) * -1;

        gren.GetComponent<Rigidbody>().AddForce(startPoint.forward * (throwForce), ForceMode.Impulse);
    }
}