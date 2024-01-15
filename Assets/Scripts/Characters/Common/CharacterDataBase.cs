using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class CharacterDataBase
{
    protected const string GROUP_INFO = "[Info]";
    protected const string GROUP_STATUS = "[Status]";
    protected const string GROUP_SPEED = "[Speed]";

    // 캐릭터 이름
    [BoxGroup(GROUP_INFO), SerializeField]
    private string _charName;

    // 캐릭터 체력 / 스테미나
    [BoxGroup(GROUP_STATUS), SerializeField]
    private int _charMaxHP;
    [BoxGroup(GROUP_STATUS), SerializeField]
    private int _charCurrHP;
    [BoxGroup(GROUP_STATUS), SerializeField]
    private float _charStamina;

    // 캐릭터 공격력 / 방어력
    [BoxGroup(GROUP_STATUS), SerializeField]
    private int _offensivePower;
    [BoxGroup(GROUP_STATUS), SerializeField]
    private int _defensivePower;

    // 캐릭터 이동 속도
    [BoxGroup(GROUP_SPEED), SerializeField]
    private float _walkSpeed;
    [BoxGroup(GROUP_SPEED), SerializeField]
    private float _sprintSpeed;

    // 프로퍼티
    public string ThisName
    {
        get { return _charName; }
        set { _charName = value; }
    }

    public int ThisMaxHP
    {
        get { return _charMaxHP; }
        set { _charMaxHP = value; }
    }
    public int ThisCurrHP
    {
        get { return _charCurrHP; }
        set { _charCurrHP = value; }
    }
    public float ThisStamina
    {
        get { return _charStamina; }
        set { _charStamina = value; }
    }

    public int ThisOffensivePower
    {
        get { return _offensivePower; }
        set { _offensivePower = value; }
    }
    public int ThisDefensivePower
    {
        get { return _defensivePower; }
        set { _defensivePower = value; }
    }

    public float ThisWalkSpeed
    {
        get { return _walkSpeed; }
        set { _walkSpeed = value; }
    }
    public float ThisSprintSpeed
    {
        get { return _sprintSpeed; }
        set { _sprintSpeed = value; }
    }
}
