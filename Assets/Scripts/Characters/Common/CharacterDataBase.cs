using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterDataBase
{
    // 캐릭터 이름
    private string _charName;

    // 캐릭터 체력 / 스테미나
    private int _charHP;
    private float _charStamina;

    // 캐릭터 공격력 / 방어력
    private int _offensivePower;
    private int _defensivePower;

    // 캐릭터 이동 속도
    private float _applySpeed;
    private float _walkSpeed;
    private float _sprintSpeed;

    // 프로퍼티
    public string ThisName => _charName;

    public int ThisHP => _charHP;
    public float ThisStamina => _charStamina;

    public int ThisOffensivePower => _offensivePower;
    public int ThisDefensivePower => _defensivePower;

    public float ThisWalkSpeed => _walkSpeed;
    public float ThisSprintSpeed => _sprintSpeed;

    public void SetCharacterName(string name)
    {
        _charName = name;
    }

    public void SetCharacterHP(int hp)
    {
        _charHP = hp;
    }

    public void SetCharacterStamina(float stamina)
    {
        _charStamina = stamina;
    }

    public void SetCharacterOppensivePower(int offensivePower)
    {
        _offensivePower = offensivePower;
    }

    public void SetCharacterDefensivePower(int defensivePower)
    {
        _defensivePower = defensivePower;
    }
}
