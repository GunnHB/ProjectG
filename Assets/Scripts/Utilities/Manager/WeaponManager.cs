using System.Collections.Generic;

using UnityEngine;

public class WeaponManager : SingletonObject<WeaponManager>
{
    private PlayerAnimCtrlScriptableObject _animSO;

    private Dictionary<HandPosition, ItemWeaponBase> _currWeaponInfo = new();

    protected override void Awake()
    {
        base.Awake();

        _animSO = Resources.Load<PlayerAnimCtrlScriptableObject>("ScriptableObject/PlayerAnimCtrlSO");
    }

    private void InitWeaponInfo()
    {

    }

    // 장비 착용
    public void EquipWeapon(Weapon.Data data, GameObject obj)
    {
        if (GameManager.Instance.PController == null)
            return;

        // 프리팹을 생성
        GameObject itemObj = null;

        if (data.type == WeaponType.OneHand || data.type == WeaponType.Arrow)
            itemObj = Instantiate(obj, GameManager.Instance.PController.RightHand);
        else
            itemObj = Instantiate(obj, GameManager.Instance.PController.LeftHand);

        if (itemObj != null)
        {
            var item = itemObj.GetComponent<ItemBase>();

            if (item != null)
                item.ThisItemData.SetEquip(true);
        }

        SetPlayerAnim(data);
    }

    // 장비 해제
    public void UnequipWeapon(Item.Data data)
    {
        if (GameManager.Instance.PController == null)
            return;
    }

    // 앞 선 무기의 여부에 따라 방패, 화살은 애니가 약간 달라질 필요가 있음
    private void SetPlayerAnim(Weapon.Data data)
    {
        var pController = GameManager.Instance.PController;

        if (pController == null || _animSO == null)
            return;

        switch (data.type)
        {
            case WeaponType.None:
            case WeaponType.NoWeapon:
                pController.PlayerAnimator.runtimeAnimatorController = _animSO._playerAnimCtrlDic[WeaponType.NoWeapon];
                break;
            case WeaponType.OneHand:
                pController.PlayerAnimator.runtimeAnimatorController = _animSO._playerAnimCtrlDic[WeaponType.OneHand];
                break;
            case WeaponType.TwoHand:
                pController.PlayerAnimator.runtimeAnimatorController = _animSO._playerAnimCtrlDic[WeaponType.TwoHand];
                break;
            case WeaponType.Shield:
                pController.PlayerAnimator.runtimeAnimatorController = _animSO._playerAnimCtrlDic[WeaponType.OneHand];
                break;
            case WeaponType.Bow:
                pController.PlayerAnimator.runtimeAnimatorController = _animSO._playerAnimCtrlDic[WeaponType.Bow];
                break;
            case WeaponType.Arrow:
                pController.PlayerAnimator.runtimeAnimatorController = _animSO._playerAnimCtrlDic[WeaponType.Bow];
                break;
        }
    }
}
