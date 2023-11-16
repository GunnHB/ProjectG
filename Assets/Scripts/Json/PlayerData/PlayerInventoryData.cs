using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerInventoryData
{
    // 인벤토리는 가변형이라 리스트
    public Dictionary<SlotIndex, Dictionary<InventoryCategory, List<Item.Data>>> _playerInventory;
    public Dictionary<SlotIndex, int> _playerGold;
    public Dictionary<SlotIndex, Dictionary<InventoryCategory, int>> _invenCateSize;

    public PlayerInventoryData()
    {
        _playerInventory = new();
        _playerGold = new();
        _invenCateSize = new();

        var tempCateSzie = new Dictionary<InventoryCategory, int>();
        var tempInven = new Dictionary<InventoryCategory, List<Item.Data>>();

        // 노가다 느낌......
        tempCateSzie.Add(InventoryCategory.CategoryWeapon, GameValue.INVENTORY_DEFAULT_CATE_WEAPON_SIZE);
        tempCateSzie.Add(InventoryCategory.CategoryArmor, GameValue.INVENTORY_DEFAULT_CATE_ARMOR_SIZE);
        tempCateSzie.Add(InventoryCategory.CategoryShield, GameValue.INVENTORY_DEFAULT_CATE_SHIELD_SIZE);
        tempCateSzie.Add(InventoryCategory.CategoryBow, GameValue.INVENTORY_DEFAULT_CATE_BOW_SIZE);
        tempCateSzie.Add(InventoryCategory.CategoryFood, GameValue.INVENTORY_DEFAULT_CATE_FOOD_SIZE);
        tempCateSzie.Add(InventoryCategory.CategoryDefault, GameValue.INVENTORY_DEFAULT_CATE_DEFAULT_SIZE);

        tempInven.Add(InventoryCategory.CategoryWeapon, new());
        tempInven.Add(InventoryCategory.CategoryArmor, new());
        tempInven.Add(InventoryCategory.CategoryShield, new());
        tempInven.Add(InventoryCategory.CategoryBow, new());
        tempInven.Add(InventoryCategory.CategoryFood, new());
        tempInven.Add(InventoryCategory.CategoryDefault, new());

        for (int index = 0; index < GameValue.SAVE_SLOT_COUNT; index++)
        {
            _playerInventory.Add((SlotIndex)index, tempInven);
            _invenCateSize.Add((SlotIndex)index, tempCateSzie);
            _playerGold.Add((SlotIndex)index, 0);
        }
    }
}
