using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : ItemBase
{
    private Weapon.Data _weaponData;

    public Weapon.Data WeaponData => _weaponData;

    private bool _isEquip = false;

    private void Awake()
    {
        if (_itemData != null)
            _weaponData = ItemManager.Instance.GetWeaponDataByRefId(_itemData.ref_id);

        _rigidBody.isKinematic = !_isEquip;
        _rigidBody.detectCollisions = !_isEquip;
    }
}