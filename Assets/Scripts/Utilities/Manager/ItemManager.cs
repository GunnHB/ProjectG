using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 아이템 / 인벤토리 관리는 여기서 합니다.
public class ItemManager : SingletonObject<ItemManager>
{
    protected override void Awake()
    {
        base.Awake();
    }

    // 탭 관리용
    private List<UIInventoryTab> _tabList = new();
    public List<UIInventoryTab> TabList => _tabList;

    private UIInventoryTab _currSelectTab;
    public UIInventoryTab CurrSelectTab => _currSelectTab;

    private UIItemSlot _currItemSlot;
    public UIItemSlot CurrItemSlot => _currItemSlot;

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

    public void AddToTabList(UIInventoryTab tab)
    {
        _tabList.Add(tab);
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

    // 인벤토리 닫히면 정리해주기
    public void ClearDatas()
    {
        ClearSelectCategory();
        ClearCurrItemSlot();

        _tabList.Clear();
    }
}
