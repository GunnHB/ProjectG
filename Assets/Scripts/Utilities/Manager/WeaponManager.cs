using System.Collections.Generic;

using UnityEngine;

public class WeaponManager : SingletonObject<WeaponManager>
{
    private PlayerAnimCtrlScriptableObject _animSO;
    private PlayerBaseData _baseData;

    #region 캐싱
    private string _path => JsonManager.PLAYER_DATA;
    private string _fileName => JsonManager.PLAYER_BASE_DATA_FILE_NAME;

    private Transform _rightHand
    {
        get
        {
            if (GameManager.Instance.PController == null)
                return null;
            else
                return GameManager.Instance.PController.RightHand;
        }
    }

    private Transform _leftHand
    {
        get
        {
            if (GameManager.Instance.PController == null)
                return null;
            else
                return GameManager.Instance.PController.LeftHand;
        }
    }

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
            if (IsRightWeapon(weaponData))
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

        itemObj = Instantiate(obj, IsRightWeapon(data) ? _rightHand : _leftHand);

        if (itemObj != null)
        {
            var item = itemObj.GetComponent<ItemBase>();

            if (item != null)
                item.ThisItemData.SetEquip(true);
        }

        SetPlayerAnim(data);
    }

    // 장비 해제
    public void UnequipWeapon(ItemData data)
    {
        if (GameManager.Instance.PController == null)
            return;

        data.SetEquip(false);

        var weaponData = ItemManager.Instance.GetWeaponDataByRefId(data.Data.ref_id);
        var handTransform = IsRightWeapon(weaponData) ? _rightHand : _leftHand;

        for (int index = 0; index < handTransform.childCount; index++)
        {
            var prefab = handTransform.GetChild(index);

            if (prefab != null)
                Destroy(prefab.gameObject);
        }

        if (IsRightWeapon(weaponData))
            CurrWeaponInfo[HandPosition.Right].ResetData();
        else
            CurrWeaponInfo[HandPosition.Left].ResetData();

        JsonManager.Instance.SaveData(_path, _fileName, _baseData);

        SetNoWeaponAnim();
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

    private void SetNoWeaponAnim()
    {
        if (GameManager.Instance.PController == null)
            return;

        GameManager.Instance.PController.PlayerAnimator.runtimeAnimatorController = _animSO._playerAnimCtrlDic[WeaponType.NoWeapon];
    }

    // 오른쪽이 아닐 땐 놀랍게도 왼쪽입니다.
    private bool IsRightWeapon(Weapon.Data data)
    {
        if (data.type == WeaponType.Arrow || data.type == WeaponType.OneHand)
            return true;
        else
            return false;
    }
}
