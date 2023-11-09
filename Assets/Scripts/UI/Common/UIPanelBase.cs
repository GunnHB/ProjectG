using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class UIPanelBase : UIBase
{
    protected override void Awake()
    {
        base.Awake();

        _uiType = UIBaseType.Panel;
    }
}