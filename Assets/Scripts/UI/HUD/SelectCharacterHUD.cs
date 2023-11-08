using System.Collections;
using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.UI;

using TMPro;
using System;

public class SelectCharacterHUD : UIHUDBase
{
    // private const string LeftGroup = "LeftGroup";
    // private const string CenterGroup = "CetnerGroup";
    // private const string RightGroup = "RightGroup";

    // [TabGroup(LeftGroup), SerializeField] private TextMeshProUGUI _leftPlayerName;
    // [TabGroup(LeftGroup), SerializeField] private GameObject _leftContainGroup;
    // [TabGroup(LeftGroup), SerializeField] private GameObject _leftButtonGroup;
    // [TabGroup(LeftGroup), SerializeField] private Button _leftCreateButton;
    // [TabGroup(LeftGroup), SerializeField] private Button _leftDeleteButton;
    // [TabGroup(LeftGroup), SerializeField] private Button _leftPlayButton;

    // [TabGroup(CenterGroup), SerializeField] private TextMeshProUGUI _centerPlayerName;
    // [TabGroup(CenterGroup), SerializeField] private GameObject _centerContainGroup;
    // [TabGroup(CenterGroup), SerializeField] private GameObject _centerButtonGroup;
    // [TabGroup(CenterGroup), SerializeField] private Button _centerCreateButton;
    // [TabGroup(CenterGroup), SerializeField] private Button _centerDeleteButton;
    // [TabGroup(CenterGroup), SerializeField] private Button _centerPlayButton;

    // [TabGroup(RightGroup), SerializeField] private TextMeshProUGUI _rightPlayerName;
    // [TabGroup(RightGroup), SerializeField] private GameObject _rightContainGroup;
    // [TabGroup(RightGroup), SerializeField] private GameObject _rightButtonGroup;
    // [TabGroup(RightGroup), SerializeField] private Button _rightCreateButton;
    // [TabGroup(RightGroup), SerializeField] private Button _rightDeleteButton;
    // [TabGroup(RightGroup), SerializeField] private Button _rightPlayButton;

    [Title("[ObjectPool]"), SerializeField] private ObjectPool _slotPool;

    [Title("[Back Button]"), SerializeField] private Button _backButton;

    protected override void Awake()
    {
        base.Awake();

        // // left
        // Util.AddButtonListener(_leftCreateButton, () => OnClickCreate(0));
        // Util.AddButtonListener(_leftDeleteButton, () => OnClickDelete(0));
        // Util.AddButtonListener(_leftPlayButton, () => OnClickPlay(0));

        // // right
        // Util.AddButtonListener(_rightCreateButton, () => OnClickCreate(1));
        // Util.AddButtonListener(_rightDeleteButton, () => OnClickDelete(1));
        // Util.AddButtonListener(_rightPlayButton, () => OnClickPlay(1));

        // // center
        // Util.AddButtonListener(_centerCreateButton, () => OnClickCreate(2));
        // Util.AddButtonListener(_centerDeleteButton, () => OnClickDelete(2));
        // Util.AddButtonListener(_centerPlayButton, () => OnClickPlay(2));

        _slotPool.Initialize();

        // back button
        Util.AddButtonListener(_backButton, OnClickBackButton);

        // // 일단 다 끄기
        // DeactiveContainGroup();
        // DeactiveButtonGroup();

        // // 일단 다 켜기
        // ActiveCreateButton();
    }

    // private void DeactiveContainGroup()
    // {
    //     _leftContainGroup.SetActive(false);
    //     _centerContainGroup.SetActive(false);
    //     _rightContainGroup.SetActive(false);
    // }

    // private void DeactiveButtonGroup()
    // {
    //     _leftButtonGroup.SetActive(false);
    //     _centerButtonGroup.SetActive(false);
    //     _rightButtonGroup.SetActive(false);
    // }

    // private void ActiveCreateButton()
    // {
    //     _leftCreateButton.gameObject.SetActive(true);
    //     _centerCreateButton.gameObject.SetActive(true);
    //     _rightCreateButton.gameObject.SetActive(true);
    // }

    public void Init()
    {
        _slotPool.ReturnAllObject();

        for (int index = 0; index < JsonManager.Instance.BaseSlotData._isEmpty.Length; index++)
            _slotPool.GetObject();
    }

    private void OnClickCreate(int index)
    {
        GameManager.Instance.SetSelectedSlotIndex(index);

        this.gameObject.SetActive(false);

        var openUI = UIManager.Instance.FindOpendUI<CustomizeHUD>(UIManager.Instance.HudCanvas);

        if (openUI != null)
            openUI.gameObject.SetActive(true);
        else
            openUI = UIManager.Instance.OpenUI<CustomizeHUD>("HUD/CustomizeHUD");

        openUI.Init();
    }

    private void OnClickDelete(int index)
    {
        GameManager.Instance.SetSelectedSlotIndex(index);
    }

    private void OnClickPlay(int index)
    {
        GameManager.Instance.SetSelectedSlotIndex(index);
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
