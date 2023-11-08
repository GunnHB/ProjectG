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

    public PlayerBaseData()
    {
        this._playerName = new string[GameValue.SAVE_SLOT_COUNT];

        this._playerHP = new int[GameValue.SAVE_SLOT_COUNT];
        this._playerStamina = new int[GameValue.SAVE_SLOT_COUNT];
    }
}
