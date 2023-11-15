using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerInventoryData : MonoBehaviour
{
    // 인벤토리는 가변형이라 리스트
    private Dictionary<SlotIndex, List<Item.Data>> _playerInventory;
    private Dictionary<SlotIndex, int> _playerGold;

    public PlayerInventoryData()
    {
        for (int index = 0; index < GameValue.SAVE_SLOT_COUNT; index++)
        {
            _playerInventory.Add((SlotIndex)index, new());
            _playerGold.Add((SlotIndex)index, 0);
        }
    }
}
