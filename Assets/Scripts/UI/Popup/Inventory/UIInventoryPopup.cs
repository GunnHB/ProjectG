using System.Collections;
using System.Collections.Generic;

using Sirenix.OdinInspector;

using TMPro;

using UnityEngine;

using System.Linq;
using Unity.VisualScripting;

public class UIInventoryPopup : UIPopupBase
{
    private const string UP_PANEL = "UpPanel";
    private const string RIGHT_PANEL = "RightPanel";
    private const string LEFT_PANEL = "Left_Panel";

    [Title("[Pool]")]
    [TabGroup(LEFT_PANEL), SerializeField] private ObjectPool _tabPool;
    [TabGroup(LEFT_PANEL), SerializeField] private ObjectPool _inventoryRowPool;

    [Title("[Componenets]")]
    [TabGroup(UP_PANEL), SerializeField] private TextMeshProUGUI _goldText;

    [TabGroup(RIGHT_PANEL), SerializeField] private TextMeshProUGUI _itemName;
    [TabGroup(RIGHT_PANEL), SerializeField] private TextMeshProUGUI _itemDesc;

    // cache
    private PlayerInventoryData _inventoryData;
    private SlotIndex _playerSlotIndex;

    protected override void Awake()
    {
        base.Awake();

        _tabPool.Initialize();
        _inventoryRowPool.Initialize();

        _inventoryData = JsonManager.Instance.InventoryData;
        _playerSlotIndex = (SlotIndex)GameManager.Instance.SelectedSlotIndex;
    }

    public void Init()
    {
        _tabPool.ReturnAllObject();
        _inventoryRowPool.ReturnAllObject();

        SetGold();

        InitTabs();
    }

    private void InitTabs()
    {
        var tabList = InventoryTab.Data.GetList().OrderBy(x => x.order);
        int tabIndex = 0;

        foreach (var tabData in tabList)
        {
            var temp = _tabPool.GetObject();

            if (temp.TryGetComponent(out UIInventoryTab tab))
            {
                // 이름 재정의
                temp.name = $"CateTab_{tabIndex}";

                tab.gameObject.SetActive(true);

                tab.Init(tabData);
                tab._tabAction = InitInventoryRow;

                ItemManager.Instance.AddToTabList(tab);
            }

            tabIndex++;
        }

        ItemManager.Instance.InvokeTabAction();
    }

    private void InitInventoryRow()
    {
        if (ItemManager.Instance.CurrSelectTab == null)
            return;

        int invenSize = _inventoryData._invenCateSize[_playerSlotIndex][ItemManager.Instance.CurrSelectTab.TabCategory];
        int rowCount = invenSize / GameValue.INVENTORY_ROW_AMOUNT;
        int remainCount = invenSize % GameValue.INVENTORY_ROW_AMOUNT;

        // 나머지가 있으면 한 줄 더 추가해준다.
        if (remainCount != 0)
            ++rowCount;

        // 모두 반환
        _inventoryRowPool.ReturnAllObject();

        for (int index = 0; index < rowCount; index++)
        {
            var temp = _inventoryRowPool.GetObject();

            if (temp.TryGetComponent(out UIInventoryRow row))
            {
                // 이름 재정의
                temp.name = $"InventoryRow_{index}";
                temp.SetActive(true);

                // 마지막 줄 && 나머지가 있으면 나머지 개수만큼만 초기화 해주면 됨
                int initCount = (index + 1 == rowCount && remainCount != 0) ? remainCount : GameValue.INVENTORY_ROW_AMOUNT;

                row.Init(index, initCount);
            }
        }
    }

    private void SetGold()
    {
        _goldText.text = _inventoryData._playerGold[_playerSlotIndex].AddComma();
    }

    public override void Open()
    {
        base.Open();
    }

    public override void Close()
    {
        base.Close();

        ItemManager.Instance.ClearDatas();
    }
}
