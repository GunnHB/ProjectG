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

    protected virtual void Start()
    {
        this.gameObject.transform.SetParent(UIManager.Instance.HudCanvas.transform);
    }
}
