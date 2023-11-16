using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableLoadManager : SingletonObject<TableLoadManager>
{
    // 테이블이 새로 추가되면 여기서 로드를 해줘야합니다.
    protected override void Awake()
    {
        base.Awake();

        Item.Data.Load();
        InventoryTab.Data.Load();
        Weapon.Data.Load();
    }
}
