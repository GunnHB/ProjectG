using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using Sirenix.OdinInspector;

public abstract class UIDialogueBase : UIBase
{
    protected override void Awake()
    {
        base.Awake();

        _uiType = UIBaseType.Dialogue;
    }

    public abstract void InitUIDialogue(string titleString, string messageString,
                                        string confirmString = "Confirm", UnityAction confirmAction = null,
                                        string cancelString = "Cancel", UnityAction cancelAction = null);
}
