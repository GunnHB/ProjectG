using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIInventoryRow : MonoBehaviour
{
    [SerializeField] private ObjectPool _itemSlotPool;

    private void Awake()
    {
        _itemSlotPool.Initialize();
    }

    // 각 슬롯에 콜백 함수를 추가해줌
    public void Init(int rowIndex, int count, UnityAction slotCallback)
    {
        _itemSlotPool.ReturnAllObject();
        ItemManager.Instance.ClearCurrItemSlot();                   // 선택된 슬롯을 비워준다.

        for (int index = 0; index < count; index++)
        {
            var temp = _itemSlotPool.GetObject();

            if (temp.TryGetComponent(out UIItemSlot itemSlot))
            {
                temp.name = $"Row_{rowIndex}_item_{index}";
                itemSlot.gameObject.SetActive(true);

                itemSlot.InitSlot();
                itemSlot._slotCallback = slotCallback;
            }
        }
    }
}
