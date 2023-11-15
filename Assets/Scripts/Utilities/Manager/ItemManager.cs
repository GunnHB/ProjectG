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
    private UIInventoryTab _newSelectTab;

    public UIInventoryTab CurrSelectTab => _currSelectTab;
    public UIInventoryTab NewSelectTab => _newSelectTab;

    public void SetPrevSelectTab(UIInventoryTab tab)
    {
        _currSelectTab = tab;
    }

    public void ChangeSelectCategory(UIInventoryTab newCate)
    {
        _newSelectTab = newCate;

        // 이미 선택된 탭이라 굳이...
        if (_currSelectTab == _newSelectTab)
            return;

        // 새롭게 선택된 탭을 켜고 이전 탭은 끈다.
        _currSelectTab.SetSelect(false);
        _newSelectTab.SetSelect(true);

        _currSelectTab = _newSelectTab;
        _newSelectTab = null;

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

    // 인벤토리 닫히면 정리해주기
    public void ClearDatas()
    {
        _currSelectTab = null;
        _newSelectTab = null;

        _tabList.Clear();
    }
}
