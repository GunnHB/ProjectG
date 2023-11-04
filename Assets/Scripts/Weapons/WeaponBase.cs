using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 무기의 기본적인 정보를 담는 클래스
/// </summary>
public class WeaponBase : MonoBehaviour
{
    public enum WeaponType
    {
        None,
        NoWeapon,
        SwordShield,
        Bow,
        SingleTwoHandSword
    }

    public WeaponType _weaponType;

    public string _weaponName;

    public float _damage;
    public float _durability;

    public Animation _weaponAnimation;
    public AudioClip _weaponAudio;
}
