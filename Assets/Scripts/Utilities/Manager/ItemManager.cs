using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 아이템 관리는 여기서 합니다.
public class ItemManager : SingletonObject<ItemManager>
{
    protected override void Awake()
    {
        base.Awake();
    }
}
