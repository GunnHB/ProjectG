using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIInventoryTab : MonoBehaviour
{
    [SerializeField] private Image _tabImage;
    [SerializeField] private CommonButton _tabButton;
    [SerializeField] private TextMeshProUGUI _tabName;

    private Color _unselectColor = new Color(1f, 1f, 1f, .5f);
    private Color _enterColor = new Color(1f, 1f, 1f, .75f);
    private Color _selectColor = new Color(1f, 1f, 1f, 1f);

    private bool _select = false;

    private InventoryTab.Data _tabData;

    // 탭 클릭 시 실행될 콜백
    public UnityAction _tabAction = null;

    public bool Select => _select;
    public InventoryCategory TabCategory => _tabData.category;

    private void Awake()
    {
        _tabImage.color = _unselectColor;
        _select = false;
        _tabName.gameObject.SetActive(false);

        Util.AddHoverButtonListener(_tabButton, OnEnterTabButton, OnExitTabButton, OnClickTabButton);
    }

    // 먼저 선택되어야 할 카테고리가 있다면 값을 할당 
    public void Init(InventoryTab.Data tabData, InventoryCategory _selectTabCate = InventoryCategory.CategoryWeapon)
    {
        _tabData = tabData;

        SetTabName();
        SetTabImage();

        SetSelect(_tabData.category == _selectTabCate);

        // 선택된 카테고리로 초기화
        if (_select)
            ItemManager.Instance.SetCurrSelectTab(this);
    }

    private void SetTabName()
    {
        if (_tabData == null)
            return;

        _tabName.text = _tabData.name;
    }

    private void SetTabImage()
    {
        if (_tabData == null)
            return;

        _tabImage.sprite = ResourceManager.Instance.GetSprite($"Inventory/Icon/{_tabData.image}");
    }

    public void SetSelect(bool select)
    {
        _select = select;

        _tabImage.color = select ? _selectColor : _unselectColor;
        _tabName.gameObject.SetActive(select);
    }

    private void OnEnterTabButton()
    {
        if (!_select)
            _tabImage.color = _enterColor;
    }

    private void OnExitTabButton()
    {
        if (!_select)
            _tabImage.color = _unselectColor;
    }

    private void OnClickTabButton()
    {
        ItemManager.Instance.ChangeSelectCategory(this);
    }
}
