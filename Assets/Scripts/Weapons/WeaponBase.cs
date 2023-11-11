using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 무기의 기본적인 정보를 담는 클래스
/// </summary>
public class WeaponBase : MonoBehaviour
{
    // 추가되는 타입은 끝에 넣으시오
    public enum WeaponType
    {
        NoWeapon,
        SwordShield,
        SingleTwoHandSword,
        Bow,
    }

    // public WeaponType _weaponType;

    // public string _weaponName;

    // public float _damage;
    // public float _durability;

    // public Animation _weaponAnimation;
    // public AudioClip _weaponAudio;
}
