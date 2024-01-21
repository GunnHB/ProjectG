using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWeapon : ItemBaseV2
{
    private Weapon.Data _weaponData;

    public Weapon.Data WeaponData => _weaponData;

    private void Awake()
    {
        if (_data != null)
            _weaponData = Weapon.Data.DataMap[_data.ref_id];
    }
}
