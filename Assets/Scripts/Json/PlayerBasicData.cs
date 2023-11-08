using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerBasicData
{
    public string[] _playerName;

    public int[] _playerHP;
    public int[] _playerStamina;

    public PlayerBasicData(int index)
    {
        this._playerName = new string[index];

        this._playerHP = new int[index];
        this._playerStamina = new int[index];
    }
}
