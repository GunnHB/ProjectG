using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerBaseData
{
    public string[] _playerName;

    public int[] _playerHP;
    public int[] _playerStamina;

    public Dictionary<SlotIndex, Dictionary<HandPosition, ItemWeaponBase>> _playerWeapon = new();

    public PlayerBaseData()
    {
        this._playerName = new string[GameValue.SAVE_SLOT_COUNT];

        this._playerHP = new int[GameValue.SAVE_SLOT_COUNT];
        this._playerStamina = new int[GameValue.SAVE_SLOT_COUNT];

        var tempData = new Dictionary<HandPosition, ItemWeaponBase>();

        tempData.Add(HandPosition.Left, null);
        tempData.Add(HandPosition.Right, null);

        for (int index = 0; index < GameValue.SAVE_SLOT_COUNT; index++)
            _playerWeapon.Add((SlotIndex)index, tempData);
    }
}
