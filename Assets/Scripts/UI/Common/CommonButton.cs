using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CommonButton : Button, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool _needPress = false;
    public bool _needEnterAndExit = false;

    private bool _isPress = false;              // 버튼 눌렸는지
    private bool _isEnter = false;              // 버튼 위에 마우스 올라갔는지
    private bool _isRightClick = false;         // 마우스 오른쪽 클릭인지

    public bool IsPress => _isPress;
    public bool IsEnter => _isEnter;
    public bool IsRightClick => _isRightClick;

    public UnityAction buttonCallback = null;

    public void SetPress(bool isPress)
    {
        _isPress = isPress;
    }

    public void SetEnter(bool isEnter)
    {
        _isEnter = isEnter;
    }

    // Press
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!_needPress)
            return;

        _isPress = true;

        base.OnPointerDown(eventData);

        buttonCallback?.Invoke();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!_needPress)
            return;

        _isPress = false;

        base.OnPointerUp(eventData);
    }

    // Enter and exit
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!_needEnterAndExit)
            return;

        _isEnter = true;

        base.OnPointerEnter(eventData);

        buttonCallback?.Invoke();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!_needEnterAndExit)
            return;

        _isEnter = false;

        base.OnPointerExit(eventData);
    }

    // Click
    public override void OnPointerClick(PointerEventData eventData)
    {
        _isRightClick = false;

        if (eventData.button == PointerEventData.InputButton.Right)
            _isRightClick = true;

        onClick?.Invoke();
    }
}
