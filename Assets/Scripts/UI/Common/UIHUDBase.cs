using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHUDBase : UIBase
{
    protected override void Awake()
    {
        base.Awake();

        _uiType = UIBaseType.HUD;
    }
}
