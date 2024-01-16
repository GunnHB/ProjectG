using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

[System.Serializable]
public class EnemyDataBase : CharacterDataBase
{
    private const string GROUP_DROP_ITEM = "[Drop Item]";

    [BoxGroup(GROUP_INFO), SerializeField, PropertyOrder(-1)]
    private long _enemyId;

    [BoxGroup(GROUP_INFO), SerializeField, TextArea(3, 10)]
    private string _charDescription;

    [BoxGroup(GROUP_DROP_ITEM), SerializeField]
    private List<ItemData> _dropItemList = new();

    // 프로퍼티
    public long ThisId
    {
        get { return _enemyId; }
        set { _enemyId = value; }
    }

    public string ThisDescription
    {
        get { return _charDescription; }
        set { _charDescription = value; }
    }

    public List<ItemData> DropItemList
    {
        get { return _dropItemList; }
        set { _dropItemList = value; }
    }

    // 행동 관련 대기 시간 프로퍼티
    public float IdleWaitTime { get; set; }
    public float CurrIdelWaitTime { get; set; }

    public float AttackWaitTime { get; set; }
    public float CurrAttackWaitTime { get; set; }

    public float AlertWaitTime { get; set; }
    public float CurrAlertWaitTime { get; set; }
}
