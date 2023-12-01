using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWeaponBase : ItemBase
{
    private Weapon.Data _weaponData;

    public Weapon.Data WeaponData => _weaponData;

    private bool _isEquip = false;

    protected override void Awake()
    {
        base.Awake();

        if (_itemData != null)
            _weaponData = ItemManager.Instance.GetWeaponDataByRefId(_itemData.ref_id);
    }
}