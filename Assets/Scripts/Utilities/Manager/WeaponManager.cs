using System.Collections.Generic;

using UnityEngine;

public class WeaponManager : SingletonObject<WeaponManager>
{
    private PlayerAnimCtrlScriptableObject _animSO;
    private PlayerBaseData _baseData;

    #region 캐싱
    private string _path => JsonManager.PLAYER_DATA;
    private string _fileName => JsonManager.PLAYER_BASE_DATA_FILE_NAME;

    public Dictionary<HandPosition, ItemData> CurrWeaponInfo
    {
        get => JsonManager.Instance.BaseData._playerWeapon[(SlotIndex)GameManager.Instance.SelectedSlotIndex];
    }
    #endregion
    protected override void Awake()
    {
        base.Awake();

        _baseData = JsonManager.Instance.BaseData;

        _animSO = Resources.Load<PlayerAnimCtrlScriptableObject>("ScriptableObject/PlayerAnimCtrlSO");

        InitWeaponInfo();
    }

    private void InitWeaponInfo()
    {
        if (GameManager.Instance.PController == null)
            return;

        if (CurrWeaponInfo.TryGetValue(HandPosition.Left, out ItemData leftItem))
            EquipWeapon(leftItem);

        if (CurrWeaponInfo.TryGetValue(HandPosition.Right, out ItemData rightData))
            EquipWeapon(rightData);
    }

    public void EquipWeapon(ItemData itemData, bool needSave = false)
    {
        var weaponData = ItemManager.Instance.GetWeaponDataByRefId(itemData.Data.ref_id);
        var prefab = ResourceManager.Instance.GetWeaponPrefab<GameObject>(itemData.Data.prefab_name);

        itemData.SetEquip(true);

        if (needSave)
        {
            if (weaponData.type == WeaponType.Arrow || weaponData.type == WeaponType.OneHand)
                CurrWeaponInfo[HandPosition.Right] = itemData;
            else
                CurrWeaponInfo[HandPosition.Left] = itemData;

            JsonManager.Instance.SaveData(_path, _fileName, _baseData);
        }

        InstantiateWeapon(weaponData, prefab);
    }

    // 장비 착용
    private void InstantiateWeapon(Weapon.Data data, GameObject obj)
    {
        if (GameManager.Instance.PController == null)
            return;
        else if (data == null || obj == null)
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
