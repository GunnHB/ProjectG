using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 원거리 무기 정보
public class RangeWeapon : WeaponBase
{
    public int _range;
}

public class RangeWeaponManager : MonoBehaviour
{
    private RangeWeapon _currentWeapon;
}
