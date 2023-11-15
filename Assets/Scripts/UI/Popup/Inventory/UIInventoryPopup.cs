using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class UIInventoryPopup : UIPopupBase
{
    private const string UP_PANEL = "UpPanel";
    private const string RIGHT_PANEL = "RightPanel";
    private const string LEFT_PANEL = "Left_Panel";

    [Title("[Pool]")]
    [TabGroup(LEFT_PANEL), SerializeField] private ObjectPool _tabPool;
    [TabGroup(LEFT_PANEL), SerializeField] private ObjectPool _inventoryRowPool;

    [Title("[Componenets]")]
    [TabGroup(UP_PANEL), SerializeField] private TextMeshProUGUI _goldText;

    [TabGroup(RIGHT_PANEL), SerializeField] private TextMeshProUGUI _itemName;
    [TabGroup(RIGHT_PANEL), SerializeField] private TextMeshProUGUI _itemDesc;

    protected override void Awake()
    {
        base.Awake();

        _tabPool.Initialize();
        _inventoryRowPool.Initialize();
    }

    public override void Open()
    {
        base.Open();


    }

    public override void Close()
    {
        base.Close();


    }
}
