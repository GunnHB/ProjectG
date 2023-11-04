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
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _cancelButton;

    public void InitUIDialogue(string title, string msg, UnityAction confirmAction, UnityAction cancelAction)
    {
        _titleText.text = title;
        _msgText.text = msg;
        Util.AddButtonListener(_confirmButton, confirmAction);
        Util.AddButtonListener(_cancelButton, cancelAction);
    }
}
