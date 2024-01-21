using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 인벤토리 내의 아이템에 관련된 데이터
// 클래스 필드를 받아서 구조체로는 힘듦
public class PlayerItemData
{
    private Item.Data _data;
    private int _amount;
    private bool _isEquip;

    // ** 생성자
    public PlayerItemData() { }

    public PlayerItemData(Item.Data data, int amount = 1, bool isEquip = false)
    {
        this._data = data;
        this._amount = amount;
        this._isEquip = isEquip;
    }

    public Item.Data Data
    {
        get { return _data; }
        set { _data = value; }
    }

    public int Amount
    {
        get { return _amount; }
        set { _amount = value; }
    }

    public bool IsEquip
    {
        get { return _isEquip; }
        set { _isEquip = value; }
    }
}

[Serializable]
public class PlayerInventoryData
{
    public Dictionary<SlotIndex, Dictionary<InventoryCategory, List<PlayerItemData>>> _playerInventory;
    public Dictionary<SlotIndex, int> _playerGold;
    public Dictionary<SlotIndex, Dictionary<InventoryCategory, int>> _invenCateSize;

    public PlayerInventoryData()
    {
        _playerInventory = new();
        _playerGold = new();
        _invenCateSize = new();
        // _invenItemAmount = new();

        var tempCateSzie = new Dictionary<InventoryCategory, int>();
        var tempInven = new Dictionary<InventoryCategory, List<PlayerItemData>>();

        // 노가다 느낌......
        tempCateSzie.Add(InventoryCategory.CategoryWeapon, GameValue.INVENTORY_DEFAULT_CATE_WEAPON_SIZE);
        tempCateSzie.Add(InventoryCategory.CategoryArmor, GameValue.INVENTORY_DEFAULT_CATE_ARMOR_SIZE);
        tempCateSzie.Add(InventoryCategory.CategoryShield, GameValue.INVENTORY_DEFAULT_CATE_SHIELD_SIZE);
        tempCateSzie.Add(InventoryCategory.CategoryBow, GameValue.INVENTORY_DEFAULT_CATE_BOW_SIZE);
        tempCateSzie.Add(InventoryCategory.CategoryFood, GameValue.INVENTORY_DEFAULT_CATE_FOOD_SIZE);
        tempCateSzie.Add(InventoryCategory.CategoryDefault, GameValue.INVENTORY_DEFAULT_CATE_DEFAULT_SIZE);

        // AddEmptyData(InventoryCategory.CategoryWeapon, tempCateSzie[InventoryCategory.CategoryWeapon], ref tempInven);
        // AddEmptyData(InventoryCategory.CategoryArmor, tempCateSzie[InventoryCategory.CategoryArmor], ref tempInven);
        // AddEmptyData(InventoryCategory.CategoryShield, tempCateSzie[InventoryCategory.CategoryShield], ref tempInven);
        // AddEmptyData(InventoryCategory.CategoryBow, tempCateSzie[InventoryCategory.CategoryBow], ref tempInven);
        // AddEmptyData(InventoryCategory.CategoryFood, tempCateSzie[InventoryCategory.CategoryFood], ref tempInven);
        // AddEmptyData(InventoryCategory.CategoryDefault, tempCateSzie[InventoryCategory.CategoryDefault], ref tempInven);

        for (int index = 0; index < GameValue.SAVE_SLOT_COUNT; index++)
        {
            _playerInventory.Add((SlotIndex)index, tempInven);
            _invenCateSize.Add((SlotIndex)index, tempCateSzie);
            _playerGold.Add((SlotIndex)index, 0);
        }
    }

    // json 파일 생성 시에 공갈 데이터 만들기
    private void AddEmptyData(InventoryCategory cate, int count, ref Dictionary<InventoryCategory, List<PlayerItemData>> tempDic)
    {
        var list = new List<PlayerItemData>();

        for (int index = 0; index < count; index++)
            list.Add(new PlayerItemData());

        tempDic.Add(cate, list);
    }
}
