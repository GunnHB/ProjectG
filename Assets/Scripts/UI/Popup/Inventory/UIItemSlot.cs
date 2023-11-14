using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Sirenix.OdinInspector;

using TMPro;
using UniRx.Triggers;

public class ItemData
{
    public string _itemName;
    public ItemType _itemType;
}

public class UIItemSlot : MonoBehaviour
{
    [Title("[Item]")]
    [SerializeField] private Image _itmeImage;
    [SerializeField] private TextMeshProUGUI _itemAmountText;
    [SerializeField] private Image _selectFrame;
    [SerializeField] private CommonButton _slotButton;
    [SerializeField] private GameObject _equipObj;

    private ItemData _itemData;

    private Color _hoverColor = new Color(1f, 1f, 1f, .5f);
    private Color _selectColor = new Color(1f, 1f, 1f, 1f);

    private bool _isSelect = false;

    private void Awake()
    {
        Util.AddHoverButtonListener(_slotButton, OnEnterSlot, OnExitSlot, OnClickSlot);
    }

    public void InitSlot()
    {

    }

    private void OnClickSlot()
    {
        Debug.Log("클릭했지롱");

        // 선택안된 슬롯일 경우만 실행
        if (!_isSelect)
        {
            _selectFrame.gameObject.SetActive(true);
            _selectFrame.color = _selectColor;

            _isSelect = true;
        }
    }

    // 슬롯에 커서 올라갔을 때
    private void OnEnterSlot()
    {
        Debug.Log("올라왔지롱");

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
        Debug.Log("나갔지롱");

        // 선택안된 슬롯이면 걍 끔
        if (!_isSelect)
            _selectFrame.gameObject.SetActive(false);
    }
}
