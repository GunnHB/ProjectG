using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class SlotBaseData
{
    public bool[] _isEmpty;

    public SlotBaseData()
    {
        this._isEmpty = new bool[GameValue.SAVE_SLOT_COUNT];
    }
}
