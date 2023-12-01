using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class WeaponManager : SingletonObject<WeaponManager>
{
    // private WeaponBase _currentWeapon;

    public WeaponType weaponType;

    protected override void Awake()
    {
        base.Awake();
    }

    public void EquipWeapon(Weapon.Data data, GameObject obj)
    {
        if (GameManager.Instance.PController == null)
            return;

        if (data.type == WeaponType.OneHand || data.type == WeaponType.Arrow)
            Instantiate(obj, GameManager.Instance.PController.RightHand);
        else
            Instantiate(obj, GameManager.Instance.PController.LeftHand);
    }
}
