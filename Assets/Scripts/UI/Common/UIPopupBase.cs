using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopupBase : UIBase
{
    protected override void Awake()
    {
        base.Awake();

        _uiType = UIBaseType.Panel;
    }
}
