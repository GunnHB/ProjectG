using System.Collections;
using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.UI;

using TMPro;
using System;

public class SelectCharacterHUD : UIHUDBase
{
    [Title("[ObjectPool]")]
    [SerializeField] private ObjectPool _slotPool;
    [Title("[Back Button]")]
    [SerializeField] private Button _backButton;

    protected override void Awake()
    {
        base.Awake();

        // 슬롯 풀 초기화
        _slotPool.Initialize();

        // back button
        Util.AddButtonListener(_backButton, OnClickBackButton);
    }

    public void Init()
    {
        // 기존에 Dequeue 했던 것들 다시 가져오기
        _slotPool.ReturnAllObject();

        // json 배열 길이만큼 슬롯 생성
        for (int index = 0; index < JsonManager.Instance.BaseSlotData._isEmpty.Length; index++)
        {
            var item = _slotPool.GetObject();
            var isEmpty = JsonManager.Instance.BaseSlotData._isEmpty[index];

            item.name = $"SaveSlot_{index}";
            item.SetActive(true);

            if (item.TryGetComponent(out SelectSlot slot))
                slot.InitSlot((SelectSlot.SeparateType)index, ClosePanel);
        }
    }

    private void ClosePanel()
    {
        this.gameObject.SetActive(false);

        var openUI = UIManager.Instance.FindOpendUI<CustomizeHUD>(UIManager.Instance.HudCanvas);

        if (openUI != null)
            openUI.gameObject.SetActive(true);
        else
            openUI = UIManager.Instance.OpenUI<CustomizeHUD>("HUD/CustomizeHUD");

        openUI.Init();
    }

    private void OnClickBackButton()
    {
        this.gameObject.SetActive(false);

        var openUI = UIManager.Instance.FindOpendUI<UITitlePanel>(UIManager.Instance.PanelCanvas);

        if (openUI != null)
            openUI.gameObject.SetActive(true);
        else
            openUI = UIManager.Instance.OpenUI<UITitlePanel>("Panel/UITitlePanel");

        openUI.Init();
    }
}
