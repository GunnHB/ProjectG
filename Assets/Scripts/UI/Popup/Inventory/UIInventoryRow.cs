using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryRow : MonoBehaviour
{
    [SerializeField] private ObjectPool _itemSlotPool;

    private void Awake()
    {
        _itemSlotPool.Initialize();
    }

    public void Init(int rowIndex, int count)
    {
        _itemSlotPool.ReturnAllObject();
        // 큐에 있는 슬롯의 데이터를 초기화해줘야 함!!!

        for (int index = 0; index < count; index++)
        {
            var temp = _itemSlotPool.GetObject();

            if (temp.TryGetComponent(out UIItemSlot itemSlot))
            {
                temp.name = $"Row_{rowIndex}_item_{index}";
                temp.gameObject.SetActive(true);
            }
        }
    }
}
