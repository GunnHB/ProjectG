using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterDataBase
{
    // 캐릭터 이름
    protected string _charName;

    // 캐릭터 체력 / 스테미나
    protected int _charHP;
    protected float _charStamina;

    // 캐릭터 공격력 / 방어력
    protected int _oppensivePower;
    protected int _defensivePower;

    // 캐릭터 이동 속도
    protected float _applySpeed;
    protected float _walkSpeed;
    protected float _sprintSpeed;
}
