using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerInventoryData
{
    public Dictionary<SlotIndex, Dictionary<InventoryCategory, List<Item.Data>>> _playerInventory;
    public Dictionary<SlotIndex, int> _playerGold;
    public Dictionary<SlotIndex, Dictionary<InventoryCategory, int>> _invenCateSize;
    public Dictionary<SlotIndex, Dictionary<long, int>> _invenItemAmount;   // typeconverter 생성해야됨...

    public PlayerInventoryData()
    {
        _playerInventory = new();
        _playerGold = new();
        _invenCateSize = new();
        _invenItemAmount = new();

        var tempCateSzie = new Dictionary<InventoryCategory, int>();
        var tempInven = new Dictionary<InventoryCategory, List<Item.Data>>();

        // 노가다 느낌......
        tempCateSzie.Add(InventoryCategory.CategoryWeapon, GameValue.INVENTORY_DEFAULT_CATE_WEAPON_SIZE);
        tempCateSzie.Add(InventoryCategory.CategoryArmor, GameValue.INVENTORY_DEFAULT_CATE_ARMOR_SIZE);
        tempCateSzie.Add(InventoryCategory.CategoryShield, GameValue.INVENTORY_DEFAULT_CATE_SHIELD_SIZE);
        tempCateSzie.Add(InventoryCategory.CategoryBow, GameValue.INVENTORY_DEFAULT_CATE_BOW_SIZE);
        tempCateSzie.Add(InventoryCategory.CategoryFood, GameValue.INVENTORY_DEFAULT_CATE_FOOD_SIZE);
        tempCateSzie.Add(InventoryCategory.CategoryDefault, GameValue.INVENTORY_DEFAULT_CATE_DEFAULT_SIZE);

        AddEmptyData(InventoryCategory.CategoryWeapon, tempCateSzie[InventoryCategory.CategoryWeapon], ref tempInven);
        AddEmptyData(InventoryCategory.CategoryArmor, tempCateSzie[InventoryCategory.CategoryArmor], ref tempInven);
        AddEmptyData(InventoryCategory.CategoryShield, tempCateSzie[InventoryCategory.CategoryShield], ref tempInven);
        AddEmptyData(InventoryCategory.CategoryBow, tempCateSzie[InventoryCategory.CategoryBow], ref tempInven);
        AddEmptyData(InventoryCategory.CategoryFood, tempCateSzie[InventoryCategory.CategoryFood], ref tempInven);
        AddEmptyData(InventoryCategory.CategoryDefault, tempCateSzie[InventoryCategory.CategoryDefault], ref tempInven);

        for (int index = 0; index < GameValue.SAVE_SLOT_COUNT; index++)
        {
            _playerInventory.Add((SlotIndex)index, tempInven);
            _invenCateSize.Add((SlotIndex)index, tempCateSzie);
            _playerGold.Add((SlotIndex)index, 0);
            _invenItemAmount.Add((SlotIndex)index, new());
        }
    }

    // json 파일 생성 시에 공갈 데이터 만들기
    private void AddEmptyData(InventoryCategory cate, int count, ref Dictionary<InventoryCategory, List<Item.Data>> tempDic)
    {
        var list = new List<Item.Data>();

        for (int index = 0; index < count; index++)
            list.Add(new Item.Data());

        tempDic.Add(cate, list);
    }
}
