using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

[System.Serializable]
public class EnemyDataBase : CharacterDataBase
{
    private const string GROUP_DROP_ITEM = "[Drop Item]";

    [BoxGroup(GROUP_DROP_ITEM), SerializeField]
    private List<ItemData> _dropItemList = new();

    // 프로퍼티
    public List<ItemData> DropItemList
    {
        get { return _dropItemList; }
        set { _dropItemList = value; }
    }
}
