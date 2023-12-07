using System.Linq;
using System.Collections.Generic;

using UnityEngine;

// 아이템 / 인벤토리 관리는 여기서 합니다.
public class ItemManager : SingletonObject<ItemManager>
{
    private UIInventoryTab _currSelectTab;
    public UIInventoryTab CurrSelectTab => _currSelectTab;

    private UIItemSlot _currItemSlot;
    public UIItemSlot CurrItemSlot => _currItemSlot;

    // 장착된 아이템 슬롯 리스트
    private List<UIItemSlot> _equipedItemSlotList = new();
    public List<UIItemSlot> EquipedItemSlotList => _equipedItemSlotList;

    #region 캐싱
    private string _path = JsonManager.PLAYER_DATA;
    private string _fileName = JsonManager.PLAYER_INVENTORY_DATA_FILE_NAME;

    private UIInventoryPopup _inventoryPopup;
    public UIInventoryPopup InventoryPopup => _inventoryPopup;

    private PlayerInventoryData _inventoryData;

    public Dictionary<InventoryCategory, List<ItemData>> PlayerInventory
    {
        get => _inventoryData._playerInventory[(SlotIndex)GameManager.Instance.SelectedSlotIndex];
    }

    public int PlayerGold
    {
        get => _inventoryData._playerGold[(SlotIndex)GameManager.Instance.SelectedSlotIndex];
    }

    public Dictionary<InventoryCategory, int> InvenCateSize
    {
        get => _inventoryData._invenCateSize[(SlotIndex)GameManager.Instance.SelectedSlotIndex];
    }

    // public Dictionary<long, int> InvenItemAmount
    // {
    //     get => _inventoryData._invenItemAmount[(SlotIndex)GameManager.Instance.SelectedSlotIndex];
    // }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        _inventoryData = JsonManager.Instance.InventoryData;
    }

    // 인벤토리는 요걸로 엽시다.
    public void OpenInventory()
    {
        var popup = UIManager.Instance.FindOpendUI<UIInventoryPopup>();

        if (popup != null)
        {
            _inventoryPopup = popup;
            Debug.Log("이미 이쓰요");

            return;
        }

        _inventoryPopup = UIManager.Instance.OpenUI<UIInventoryPopup>("Inventory/InventoryPopup");

        if (_inventoryPopup)
            _inventoryPopup.Init();
    }

    public void ChangeSelectItemSlot(UIItemSlot newSlot)
    {
        if (_currItemSlot != null)
        {
            // 이미 선택된 슬롯이라 굳이...
            if (_currItemSlot == newSlot)
                return;

            // 새롭게 선택된 슬롯을 켜고 이전 슬롯은 끈다.
            _currItemSlot.SetSelect(false);
            newSlot.SetSelect(true);

            _currItemSlot = newSlot;
        }
        else
        {
            // 이전에 아무 슬롯도 선택되지 않았다면
            // 바로 선택 슬롯으로 세팅
            _currItemSlot = newSlot;
            _currItemSlot.SetSelect(true);
        }

        // 선택된 슬롯의 콜백을 호출
        _currItemSlot._slotCallback?.Invoke();
    }

    public void SetCurrSelectTab(UIInventoryTab tab)
    {
        _currSelectTab = tab;
    }

    public void ChangeSelectCategory(UIInventoryTab newCate)
    {
        // 이미 선택된 탭이라 굳이...
        if (_currSelectTab == newCate)
            return;

        // 새롭게 선택된 탭을 켜고 이전 탭은 끈다.
        _currSelectTab.SetSelect(false);
        newCate.SetSelect(true);

        _currSelectTab = newCate;

        // 선택된 탭의 콜백을 호출
        _currSelectTab._tabAction?.Invoke();
    }

    // 인벤토리 처음 열렸을 때에만 실행됨
    // 이후에는 탭의 버튼 리스너로 실행
    public void InvokeTabAction()
    {
        _currSelectTab._tabAction?.Invoke();
    }

    public void ClearCurrItemSlot()
    {
        _currItemSlot = null;
    }

    public void ClearSelectCategory()
    {
        _currSelectTab = null;
    }

    public void AddItemToInventory(ItemData item = null)
    {
        if (item == null)
            return;

        // 해당 아이템의 카테고리 얻기
        var cate = GetItemCateory(item._data);

        // 누적 가능한 아이템이라면
        if (item._data.stackable)
        {
            var hasItem = PlayerInventory[cate].Find(x => x._data.id == item._data.id);

            // 해당 아이템이 인벤토리에 있음
            if (hasItem != null)
                // InvenItemAmount[hasItem.Data.id]++;
                hasItem.AddItemAmount();
            // 해당 아이템이 인벤토리에 없음
            else
                ActualAddItem(cate, item);
        }
        // 누적 불가한 아이템이라면
        else
            ActualAddItem(cate, item);

        JsonManager.Instance.SaveData(_path, _fileName, _inventoryData);

        Debug.Log("아이템 먹어쓰요");
    }

    // 필드에 있는 아이템을 주울 때 해당 아이템은 사라져야하므로
    public void DestoryItem(ItemBase item)
    {
        // if (item != null)
        //     item.PickUpItem();
    }

    private void ActualAddItem(InventoryCategory cate, ItemData item)
    {
        // 빈 슬롯이 있는지 확인
        var emptySlotIndex = PlayerInventory[cate].FindIndex(x => x._data.id == 0);

        if (emptySlotIndex == -1)
        {
            Debug.Log("아이템 추가할 수 업쓰요");
            return;
        }
        else
        {
            PlayerInventory[cate][emptySlotIndex] = item;

            // // 수량 데이터 추가
            // if (InvenItemAmount.ContainsKey(item.Data.id))
            //     InvenItemAmount.Remove(item.Data.id);

            // InvenItemAmount.Add(item.Data.id, 1);
        }
    }

    private InventoryCategory GetItemCateory(Item.Data item)
    {
        switch (item.type)
        {
            case ItemType.Weapon:
                if (item.ref_id != 0)
                {
                    var weaponItem = Weapon.Data.DataList.Where(x => x.id == item.ref_id).FirstOrDefault();

                    if (weaponItem != null)
                    {
                        if (weaponItem.type == WeaponType.OneHand || weaponItem.type == WeaponType.TwoHand)
                            return InventoryCategory.CategoryWeapon;
                        else if (weaponItem.type == WeaponType.Shield)
                            return InventoryCategory.CategoryShield;
                        else if (weaponItem.type == WeaponType.Bow)
                            return InventoryCategory.CategoryBow;
                    }
                }
                break;
            case ItemType.Armor:
                break;
            case ItemType.Food:
                break;
            case ItemType.Default:
                break;
        }

        return InventoryCategory.CategoryArmor;
    }

    public Weapon.Data GetWeaponDataByRefId(long refId)
    {
        return Weapon.Data.GetList().Find(x => x.id == refId);
    }

    public void UseItem()
    {
        if (_currItemSlot.IsNullData || GameManager.Instance.PController == null)
            return;

        switch (_currItemSlot.ItemData._data.type)
        {
            case ItemType.Weapon:
                if (!_currItemSlot.ItemData._isEquip)
                    WeaponManager.Instance.EquipWeapon(_currItemSlot.ItemData, true);
                else
                    WeaponManager.Instance.UnequipWeapon(_currItemSlot.ItemData);
                break;
            case ItemType.Armor:
                EquipArmor();
                break;
            case ItemType.Food:
                break;
            case ItemType.Default:
                break;
        }

        JsonManager.Instance.SaveData(_path, _fileName, _inventoryData);

        _currItemSlot.RefreshSlot();
    }

    private void EquipArmor()
    {

    }

    public void DiscardItem()
    {
        if (_currItemSlot.IsNullData)
            return;

        string title = "UI_TEXT_WARNING";
        string message = "UI_ITEM_NOTICE_DISCARD";

        UIManager.Instance.OpenSimpleDialogue(title, message, confirmAction: ActualDiscardItem);
    }

    // 실제 아이템 버리기 실행
    private void ActualDiscardItem()
    {

    }

    // EquipedItemSlotList 에 있는 슬롯들 중 신규 데이터와 비교하여
    // 같은 타입의 슬롯이 있다면 신규로 교체
    // 없으면 리스트에 추가만 해줌
    public void ChangeEquipedItemSlot(ItemData newItemData)
    {
        if (!newItemData._isEquip)
            return;

        if (newItemData._data.type == ItemType.Weapon)
            ChangeEquipedByWeapon(newItemData);
        else if (newItemData._data.type == ItemType.Armor)
            ChangeEquipedByArmor(newItemData);
    }

    public void ChangeEquipedByWeapon(ItemData newItemData)
    {
        if (!newItemData._isEquip)
            return;

        for (int index = 0; index < _equipedItemSlotList.Count; index++)
        {
            var oldSlot = _equipedItemSlotList[index];
        }
    }

    public void ChangeEquipedByArmor(ItemData newItemData)
    {

    }

    // 인벤토리 닫히면 정리해주기
    public void ClearDatas()
    {
        ClearSelectCategory();
        ClearCurrItemSlot();
    }
}
