using System.Collections;
using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.UI;

using TMPro;
using System;

public class UISelectCharacterPanel : UIPanelBase
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
        for (int index = 0; index < JsonManager.Instance.SlotBaseData._isEmpty.Length; index++)
        {
            var item = _slotPool.GetObject();
            var isEmpty = JsonManager.Instance.SlotBaseData._isEmpty[index];

            item.name = $"SaveSlot_{index}";
            item.SetActive(true);

            if (item.TryGetComponent(out SelectSlot slot))
                slot.InitSlot((SelectSlot.SeparateType)index, CreateAction);
        }
    }

    private void CreateAction()
    {
        LoadSceneManager.Instance.FadeInOut(CloseAction);
    }

    private void CloseAction()
    {
        this.gameObject.SetActive(false);

        var openUI = UIManager.Instance.FindOpendUI<UICustomizePanel>(UIManager.Instance.PanelCanvas);

        if (openUI != null)
            openUI.gameObject.SetActive(true);
        else
            openUI = UIManager.Instance.OpenUI<UICustomizePanel>("CustomizePanel/UICustomizePanel");

        openUI.Init();
    }

    private void OnClickBackButton()
    {
        LoadSceneManager.Instance.FadeInOut(BackButtonAction);
    }

    private void BackButtonAction()
    {
        this.gameObject.SetActive(false);

        var openUI = UIManager.Instance.FindOpendUI<UITitlePanel>(UIManager.Instance.PanelCanvas);

        if (openUI != null)
            openUI.gameObject.SetActive(true);
        else
            openUI = UIManager.Instance.OpenUI<UITitlePanel>("TitlePanel/UITitlePanel");

        openUI.Init();
    }
}
