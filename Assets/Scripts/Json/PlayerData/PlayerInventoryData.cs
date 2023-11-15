using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerInventoryData
{
    // 인벤토리는 가변형이라 리스트
    public Dictionary<SlotIndex, List<Item.Data>> _playerInventory;
    public Dictionary<SlotIndex, int> _playerGold;
    public Dictionary<SlotIndex, Dictionary<InventoryCategory, int>> _invenCateSize;

    public PlayerInventoryData()
    {
        _playerInventory = new();
        _playerGold = new();
        _invenCateSize = new();

        var temp = new Dictionary<InventoryCategory, int>();

        // 노가다 느낌......
        temp.Add(InventoryCategory.CategoryWeapon, GameValue.INVENTORY_DEFAULT_CATE_WEAPON_SIZE);
        temp.Add(InventoryCategory.CategoryArmor, GameValue.INVENTORY_DEFAULT_CATE_ARMOR_SIZE);
        temp.Add(InventoryCategory.CategoryShield, GameValue.INVENTORY_DEFAULT_CATE_SHIELD_SIZE);
        temp.Add(InventoryCategory.CategoryBow, GameValue.INVENTORY_DEFAULT_CATE_BOW_SIZE);
        temp.Add(InventoryCategory.CategoryFood, GameValue.INVENTORY_DEFAULT_CATE_FOOD_SIZE);
        temp.Add(InventoryCategory.CategoryDefault, GameValue.INVENTORY_DEFAULT_CATE_DEFAULT_SIZE);

        for (int index = 0; index < GameValue.SAVE_SLOT_COUNT; index++)
        {
            _playerInventory.Add((SlotIndex)index, new());
            _playerGold.Add((SlotIndex)index, 0);

            _invenCateSize.Add((SlotIndex)index, temp);
        }
    }
}
