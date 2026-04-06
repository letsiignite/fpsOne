
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[System.Serializable]
public struct GunDetails {
    public GameObject gun;
    public string gunName;
    public Sprite gunImage;
};

public class WeaponSelection : MonoBehaviour
{

	[SerializeField] private int selectedWeapon = -1;
	[SerializeField] private int numberOfSlots;
	[SerializeField] private GunDetails[] gunDetails;
	[SerializeField] private GameObject GunPickupOptionPopup;
    [SerializeField] private Image GunIcon;
    [SerializeField] private TMP_Text GunNameForPickupPopup;
	[SerializeField] private GrenadeSlot grenadeSlot;
	private bool showUnarmed = true;
	private bool keyIsPressed = false;

	private List<int> GunsInHandIndex;
	private int currentGunIndex;
	private int pickupGunIndex = 0;
	private GameObject droppedGun;
	private Gun_Controller currentGun;
	private int MAX_AMMO_TO_ADD = 15;
    private int MIN_AMMO_TO_ADD = 5;
    private void Start()
	{
        GunsInHandIndex = new List<int>();
		GunsInHandIndex.Add(1);
        GunsInHandIndex.Add(2);
		currentGunIndex = 0;
        selectedWeapon = GunsInHandIndex[currentGunIndex];
        SelectWeapon();
	}

	public void ShowPickupOption(int index, GameObject droppedGun)
	{
        if (GunsInHandIndex.Contains(index))
        {
			droppedGun.SetActive(false);
			AddAmmo(index);
			return;
        }

        GunIcon.sprite = gunDetails[index].gunImage;
        GunNameForPickupPopup.text = gunDetails[index].gunName;
        GunPickupOptionPopup.SetActive(true);
		pickupGunIndex = index;
		this.droppedGun = droppedGun;
		
    }

	public void HidePickupOption()
	{
        GunPickupOptionPopup.SetActive(false);
		droppedGun = null;
    }

	public void DropAndPickupGun(int gunIndex)
	{
		HidePickupOption();

        if (GunsInHandIndex.Count == 2)
		{
            //ThrowWeapon(gunDetails[GunsInHandIndex[currentGunIndex]].gun);
            GunsInHandIndex.RemoveAt(currentGunIndex);

        }
        GunsInHandIndex.Add(gunIndex); // need to add the gun index
        selectedWeapon = gunIndex;
        currentGunIndex = GunsInHandIndex.Count - 1;
        showUnarmed = false;
        SelectWeapon();
    }

	private void ThrowWeapon(GameObject gun)
	{
		var obj = Instantiate(gun, transform.position,Quaternion.identity, this.transform );
        Rigidbody rigidbody = gun.GetComponent<Rigidbody>();

        if (rigidbody == null)
		{
            rigidbody = gun.AddComponent<Rigidbody>();
		}
		gun.transform.parent = null;
		rigidbody.isKinematic = false;
		rigidbody.useGravity = true;
        rigidbody.AddTorque(Vector3.up * 50f, ForceMode.Impulse);
		rigidbody.AddForce(Vector3.forward * 50f, ForceMode.Impulse);
    }

	private void Update()
	{

		if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.LeftShift))
		{
			keyIsPressed = true;
		}
		else if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetKeyUp(KeyCode.LeftShift))
		{
			keyIsPressed = false;
		}

        if (Input.GetKeyDown(KeyCode.F) && GunPickupOptionPopup.activeInHierarchy)
        {
			DropAndPickupGun(pickupGunIndex);
            droppedGun.SetActive(false);
        }

        int previousSelectedWeapon = selectedWeapon;

		if (Input.GetAxis("Mouse ScrollWheel") < 0f && keyIsPressed == false)
		{
			showUnarmed = false;
			if (selectedWeapon >= transform.childCount - 1)
				selectedWeapon = 0;
			else
				selectedWeapon++;
		}
		if (Input.GetAxis("Mouse ScrollWheel") > 0f && keyIsPressed == false)
		{
			showUnarmed = false;
			if (selectedWeapon <= 0)
				selectedWeapon = transform.childCount - 1;
			else
				selectedWeapon--;
		}

		if (Input.GetKeyDown(KeyCode.Alpha1) && transform.childCount >= 1 && keyIsPressed == false || Input.GetKeyDown(KeyCode.Keypad1) && transform.childCount >= 1 && keyIsPressed == false)
		{
			if(currentGunIndex == 0)
			{
				currentGunIndex = 1;
            }
            else
            {
				currentGunIndex = 0;
            }
            selectedWeapon = GunsInHandIndex[currentGunIndex];
            showUnarmed = false;
            SelectWeapon();
			return;
        }


		if (Input.GetKeyDown(KeyCode.Alpha0) || showUnarmed == true || Input.GetKeyDown(KeyCode.Keypad0))
		{
			selectedWeapon = 0;

		}
		if (Input.GetKeyDown(KeyCode.Alpha1) && transform.childCount >= 1 && keyIsPressed == false || Input.GetKeyDown(KeyCode.Keypad1) && transform.childCount >= 1 && keyIsPressed == false)
		{
			selectedWeapon = 1;
			showUnarmed = false;

		}

		if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2 && keyIsPressed == false || Input.GetKeyDown(KeyCode.Keypad2) && transform.childCount >= 2 && keyIsPressed == false)
		{
			selectedWeapon = 2;
			showUnarmed = false;
		}

		if (Input.GetKeyDown(KeyCode.Alpha3) && transform.childCount >= 3 && keyIsPressed == false || Input.GetKeyDown(KeyCode.Keypad3) && transform.childCount >= 3 && keyIsPressed == false)
		{
			selectedWeapon = 3;
			showUnarmed = false;
		}

		if (Input.GetKeyDown(KeyCode.Alpha4) && transform.childCount >= 4 && keyIsPressed == false || Input.GetKeyDown(KeyCode.Keypad4) && transform.childCount >= 4 && keyIsPressed == false)
		{
			selectedWeapon = 4;
			showUnarmed = false;
		}

		if (Input.GetKeyDown(KeyCode.Alpha5) && transform.childCount >= 5 && keyIsPressed == false || Input.GetKeyDown(KeyCode.Keypad5) && transform.childCount >= 5 && keyIsPressed == false)
		{
			selectedWeapon = 5;
			showUnarmed = false;
		}

		if (Input.GetKeyDown(KeyCode.Alpha6) && transform.childCount >= 6 && keyIsPressed == false || Input.GetKeyDown(KeyCode.Keypad6) && transform.childCount >= 6 && keyIsPressed == false)
		{
			selectedWeapon = 6;
			showUnarmed = false;
		}
		if (numberOfSlots >= 7)
		{
			if (Input.GetKeyDown(KeyCode.Alpha7) && transform.childCount >= 7 && keyIsPressed == false || Input.GetKeyDown(KeyCode.Keypad7) && transform.childCount >= 7 && keyIsPressed == false)
			{
				selectedWeapon = 7;
				showUnarmed = false;
			}
		}
		if (previousSelectedWeapon != selectedWeapon)
		{
			SelectWeapon();
		}
	}

	void SelectWeapon()
	{
		int i = 0;
		foreach (Transform weapon in transform)
		{
			if (i == selectedWeapon)
			{
				Debug.Log(" Selected index = " + i);
				weapon.gameObject.SetActive(true);
				currentGun = weapon.GetComponent<Gun_Controller>();
				currentGun.SetIsInPlayersHand(true);
			}
			else
			{
                currentGun.SetIsInPlayersHand(false);
                weapon.GetComponent<Gun_Controller>().Deactivation();
			}
			i++;
		}
	}

	public void AddAmmo(int index)
	{
		int i = 0;
		foreach (Transform weapon in transform)
		{
			if (i == index)
			{
				int _count = Random.Range(MIN_AMMO_TO_ADD, MAX_AMMO_TO_ADD);

                Debug.Log(" +++++ Picked up AMMO = "+_count);
                weapon.GetComponent<Gun_Controller>().AddAmmoFromDroppedGun(_count);

            }
			i++;
		}
	}

	public void AddAmmoToAllGunsInHand(int count)
	{
		foreach (Transform weapon in transform)
		{
            weapon.GetComponent<Gun_Controller>().AddAmmoFromDroppedGun(count);
        }

    }

	public void AddGrenade(int count)
	{
        grenadeSlot.grenadeQuantity += count;
    }
}

		 