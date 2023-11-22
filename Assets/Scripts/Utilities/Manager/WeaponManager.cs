using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class WeaponManager : SingletonObject<WeaponManager>
{
    private WeaponBase _currentWeapon;

    public WeaponType weaponType;

    protected override void Awake()
    {
        base.Awake();
    }

    private void ChangeWeapon(WeaponType type)
    {

    }
}
