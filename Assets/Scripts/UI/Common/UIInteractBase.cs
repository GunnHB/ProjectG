using UnityEngine;

public class UIInteractBase : UIBase
{
    protected override void Awake()
    {
        base.Awake();

        _uiType = UIBaseType.Interact;
    }
}