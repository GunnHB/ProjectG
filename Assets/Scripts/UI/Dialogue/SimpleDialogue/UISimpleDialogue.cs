using System.Collections;
using System.Collections.Generic;

using Sirenix.OdinInspector;

using TMPro;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UISimpleDialogue : UIDialogueBase
{
    [Title("[Box]")]
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _msgText;
    [SerializeField] private TextMeshProUGUI _confirmButtonText;
    [SerializeField] private TextMeshProUGUI _cancelButtonText;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _cancelButton;

    public override void InitUIDialogue(string titleString, string messageString,
                                        string confirmString, UnityAction confirmAction,
                                        string cancelString, UnityAction cancelAction)
    {
        _titleText.text = titleString;
        _msgText.text = messageString;
        _confirmButtonText.text = confirmString;
        _cancelButtonText.text = cancelString;

        confirmAction += Close;
        cancelAction += Close;

        Util.AddButtonListener(_confirmButton, confirmAction);
        Util.AddButtonListener(_cancelButton, cancelAction);
    }
}
