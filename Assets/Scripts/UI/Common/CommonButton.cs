using System.Collections;
using System.Collections.Generic;
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

    public bool IsPress => _isPress;
    public bool IsEnter => _isEnter;

    public UnityAction PressCallback = null;
    public UnityAction EnterCallback = null;

    // Press
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!_needPress)
            return;

        _isPress = true;

        base.OnPointerDown(eventData);

        PressCallback?.Invoke();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!_needPress)
            return;

        _isPress = false;

        base.OnPointerUp(eventData);

        PressCallback = null;
    }

    // Enter and exit
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!_needEnterAndExit)
            return;

        _isEnter = true;

        base.OnPointerEnter(eventData);

        EnterCallback?.Invoke();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!_needEnterAndExit)
            return;

        _isEnter = false;

        base.OnPointerExit(eventData);

        EnterCallback = null;
    }
}
