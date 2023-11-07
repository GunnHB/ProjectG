using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using Sirenix.OdinInspector;

public class UIDialogueBase : UIBase
{
    [Title("[Box]")]
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _msgText;
    [SerializeField] private TextMeshProUGUI _confirmButtonText;
    [SerializeField] private TextMeshProUGUI _cancelButtonText;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _cancelButton;

    protected override void Awake()
    {
        base.Awake();

        _uiType = UIBaseType.Dialogue;
    }

    public void InitUIDialogue(string title, string msg,
                               string confirmString, UnityAction confirmAction,
                               string cancelString, UnityAction cancelAction)
    {
        _titleText.text = title;
        _msgText.text = msg;
        _confirmButtonText.text = confirmString;
        _cancelButtonText.text = cancelString;

        confirmAction += Close;
        cancelAction += Close;

        Util.AddButtonListener(_confirmButton, confirmAction);
        Util.AddButtonListener(_cancelButton, cancelAction);
    }
}
