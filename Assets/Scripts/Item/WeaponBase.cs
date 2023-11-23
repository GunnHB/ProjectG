using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : ItemBase
{
    [SerializeField]
    private Weapon.Data _weaponData;

    public Weapon.Data WeaponData => _weaponData;

    protected override void PickUpItem()
    {
        base.PickUpItem();
    }

    protected override void DestoryItem()
    {
        base.DestoryItem();
    }
}