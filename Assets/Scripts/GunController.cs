using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform weaponHold;
    public Gun[] allGuns;
    Gun equippegun;

	public void Start()
	{
        
	}

    public void EquipGun(Gun gunToEquip)
    {
        if (equippegun != null) {
            Destroy(equippegun.gameObject);
        }
        equippegun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun;
        equippegun.transform.parent = weaponHold;
    }

    public void EquipGun(int weaponIndex)
    {
        EquipGun(allGuns[weaponIndex]);
    }

    public void OnTriggerHold() {
        if (equippegun != null) {
            equippegun.OnTriggerHold();
        }
    }

    public void OnTriggerRelease() {
        if (equippegun != null)
        {
            equippegun.OnTriggerRelease();
        }

    }

    public float GunHeight {
        get {
            return weaponHold.position.y;
        }
    }

    public void Aim(Vector3 aimPoint) {
        if (equippegun != null) {
            equippegun.Aim(aimPoint);
        }
    }

    public void Reload() {
        if (equippegun != null)
        {
            equippegun.Reload();
        }
    }
}
