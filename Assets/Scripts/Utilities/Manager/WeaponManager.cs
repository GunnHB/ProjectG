using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class WeaponManager : SingletonObject<WeaponManager>
{
    private WeaponBase _currentWeapon;

    public WeaponBase.WeaponType weaponType;

    protected override void Awake()
    {
        base.Awake();
    }

    private void ChangeWeapon(WeaponBase.WeaponType type)
    {
        // var scriptable = GameManager.Instance.PController.AnimCtrlScriptableObj;
        // var controller = GameManager.Instance.PController.PlayerAnimator;

        // controller.runtimeAnimatorController = scriptable._playerAnimCtrlArray[(int)type];
    }
}
