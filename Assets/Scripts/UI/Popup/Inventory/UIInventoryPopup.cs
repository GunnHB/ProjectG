using System.Collections;
using System.Collections.Generic;

using Sirenix.OdinInspector;

using TMPro;

using UnityEngine;

using System.Linq;
using Unity.VisualScripting;
using UnityEngine.UI;
using UniRx;

public class UIInventoryPopup : UIPopupBase
{
    private const string UP_PANEL = "UpPanel";
    private const string RIGHT_PANEL = "RightPanel";
    private const string LEFT_PANEL = "LeftPanel";

    [Title("[Pool]")]
    [TabGroup(LEFT_PANEL), SerializeField] private ObjectPool _tabPool;
    [TabGroup(LEFT_PANEL), SerializeField] private ObjectPool _inventoryRowPool;
    [TabGroup(LEFT_PANEL), SerializeField] private ScrollRect _invenScrollRect;

    [Title("[Componenets]")]
    [TabGroup(UP_PANEL), SerializeField] private TextMeshProUGUI _goldText;

    [TabGroup(RIGHT_PANEL), SerializeField] private TextMeshProUGUI _itemName;
    [TabGroup(RIGHT_PANEL), SerializeField] private TextMeshProUGUI _itemDesc;
    [Title("Buttons")]
    [TabGroup(RIGHT_PANEL), SerializeField] private GameObject _buttonGroup;
    [TabGroup(RIGHT_PANEL), SerializeField] private Button _useButton;
    [TabGroup(RIGHT_PANEL), SerializeField] private TextMeshProUGUI _useText;
    [TabGroup(RIGHT_PANEL), SerializeField] private Button _discardButton;
    [TabGroup(RIGHT_PANEL), SerializeField] private TextMeshProUGUI _discardText;

    // // cache
    // private PlayerInventoryData _inventoryData;
    // private SlotIndex _playerSlotIndex;

    public ScrollRect InvenScrollRect => _invenScrollRect;

    protected override void Awake()
    {
        base.Awake();

        _tabPool.Initialize();
        _inventoryRowPool.Initialize();

        _useButton.onClick.AddListener(ItemManager.Instance.UseItem);
        _discardButton.onClick.AddListener(ItemManager.Instance.DiscardItem);
    }

    public void Init()
    {
        _tabPool.ReturnAllObject();
        _inventoryRowPool.ReturnAllObject();

        _buttonGroup.gameObject.SetActive(false);

        SetGold();

        InitTabs();
    }

    private void InitTabs()
    {
        var tabList = InventoryTab.Data.DataList.OrderBy(x => x.order);
        int tabIndex = 0;

        foreach (var tabData in tabList)
        {
            var temp = _tabPool.GetObject();

            if (temp.TryGetComponent(out UIInventoryTab tab))
            {
                // 이름 재정의
                temp.name = $"CateTab_{tabIndex}";

                tab.Init(tabData);
                tab._tabAction = InitInventoryRow;
            }

            tabIndex++;
        }

        ItemManager.Instance.InvokeTabAction();
    }

    private void InitInventoryRow()
    {
        if (ItemManager.Instance.CurrSelectTab == null)
            return;

        int invenSize = ItemManager.Instance.InvenCateSize[ItemManager.Instance.CurrSelectTab.TabCategory];
        int rowCount = invenSize / GameValue.INVENTORY_ROW_AMOUNT;
        int remainCount = invenSize % GameValue.INVENTORY_ROW_AMOUNT;

        // 나머지가 있으면 한 줄 더 추가해준다.
        if (remainCount != 0)
            ++rowCount;

        // 모두 반환
        _inventoryRowPool.ReturnAllObject();

        ItemManager.Instance.EquipedItemSlotList.Clear();

        for (int index = 0; index < rowCount; index++)
        {
            var temp = _inventoryRowPool.GetObject();

            if (temp.TryGetComponent(out UIInventoryRow row))
            {
                // 이름 재정의
                temp.name = $"InventoryRow_{index}";

                // 마지막 줄 && 나머지가 있으면 나머지 개수만큼만 초기화 해주면 됨
                int initCount = (index + 1 == rowCount && remainCount != 0) ? remainCount : GameValue.INVENTORY_ROW_AMOUNT;

                row.Init(index, initCount, SelectSlotCallback);
            }
        }

        // 인벤토리가 처음 열리거나
        // 카테고리가 변경되면 아이템 정보가 초기화되도록 호출
        SelectSlotCallback();
    }

    private void SelectSlotCallback()
    {
        if (ItemManager.Instance.CurrItemSlot == null || ItemManager.Instance.CurrItemSlot.IsNullData)
        {
            _itemName.text = string.Empty;
            _itemDesc.text = string.Empty;

            _buttonGroup.gameObject.SetActive(false);
        }
        else
        {
            _itemName.text = ItemManager.Instance.CurrItemSlot.ItemData.Data.name;
            _itemDesc.text = ItemManager.Instance.CurrItemSlot.ItemData.Data.desc;

            _buttonGroup.gameObject.SetActive(true);
        }
    }

    private void SetGold()
    {
        _goldText.text = ItemManager.Instance.PlayerGold.AddComma();
    }

    public override void Close()
    {
        base.Close();

        ItemManager.Instance.ClearDatas();
    }
}
