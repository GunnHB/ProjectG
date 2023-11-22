using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

/// <summary>
/// 무기의 기본적인 정보를 담는 클래스
/// </summary>
public class WeaponBase : SerializedMonoBehaviour
{
    protected Weapon.Data _weaponData;
    protected Collider _collider;         // 공격 시 충돌체 감지용

    // property
    public Collider ThisCollider => _collider;
}
