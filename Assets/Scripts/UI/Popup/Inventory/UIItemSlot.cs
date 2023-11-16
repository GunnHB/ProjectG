using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Sirenix.OdinInspector;

using TMPro;
using UnityEngine.Events;

public class UIItemSlot : MonoBehaviour
{
    [Title("[Item]")]
    [SerializeField] private Image _itmeImage;
    [SerializeField] private TextMeshProUGUI _itemAmountText;
    [SerializeField] private Image _selectFrame;
    [SerializeField] private CommonButton _slotButton;
    [SerializeField] private GameObject _equipObj;

    private Color _hoverColor = new Color(1f, 1f, 1f, .5f);
    private Color _selectColor = new Color(1f, 1f, 1f, 1f);

    private bool _isSelect = false;

    public UnityAction _slotCallback = null;

    private Item.Data _itemData = null;
    public Item.Data ItemData => _itemData;

    private void Awake()
    {
        Util.AddEnterButtonListener(_slotButton, OnEnterSlot, OnExitSlot, OnClickSlot,
                                    ItemManager.Instance.InventoryPopup.InvenScrollRect);
    }

    public void InitSlot(Item.Data itemData = null)
    {
        ClearData();

        _itemData = itemData;

        SetData();
    }

    // 초기화 시에 일단 데이터 비워주기
    private void ClearData()
    {
        _itemData = null;

        SetImage(false);
        SetAmountText(false);
        _equipObj.SetActive(false);

        SetSelect(false);
    }

    // 슬롯은 인벤토리에 따라 빈 슬롯 / 찬 슬롯일 수 있다.
    private void SetData()
    {
        // 데이터가 비어있으면 리턴
        if (_itemData.id == 0)
            return;

        SetImage();
        SetAmountText();
    }

    private void SetImage(bool active = true)
    {
        if (!active)
        {
            _itmeImage.gameObject.SetActive(false);
            return;
        }

        _itmeImage.sprite = ResourceManager.Instance.GetSprite($"Inventory/Item/{_itemData.image}");
        _itmeImage.gameObject.SetActive(true);
    }

    private void SetAmountText(bool active = true)
    {
        if (!active)
        {
            _itemAmountText.gameObject.SetActive(false);
            return;
        }

        _itemAmountText.gameObject.SetActive(true);
    }

    private void OnClickSlot()
    {
        ItemManager.Instance.ChangeSelectItemSlot(this);
    }

    // 슬롯에 커서 올라갔을 때
    private void OnEnterSlot()
    {
        // 선택안된 슬롯일 경우만 실행
        if (!_isSelect)
        {
            _selectFrame.gameObject.SetActive(true);
            _selectFrame.color = _hoverColor;
        }
    }

    // 슬롯에서 커서 빠져 나왔을 때
    private void OnExitSlot()
    {
        // 선택안된 슬롯이면 걍 끔
        if (!_isSelect)
            _selectFrame.gameObject.SetActive(false);
    }

    public void SetSelect(bool select)
    {
        _isSelect = select;

        _selectFrame.gameObject.SetActive(_isSelect);
    }
}
