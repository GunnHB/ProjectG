using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

[System.Serializable]
public class ItemDatabase
{
    protected const string GROUP_INFO = "[INFO]";

    [BoxGroup(GROUP_INFO), SerializeField]
    protected long _itemId;
    [BoxGroup(GROUP_INFO), SerializeField]
    protected string _itemName;
    [BoxGroup(GROUP_INFO), SerializeField]
    protected string _itemDescription;
    [BoxGroup(GROUP_INFO), SerializeField]
    protected Sprite _itemSprite;

    public long ThisItemId
    {
        get { return _itemId; }
        set { _itemId = value; }
    }

    public string ThisItemName
    {
        get { return _itemName; }
        set { _itemName = value; }
    }

    public string ThisItemDesc
    {
        get { return _itemDescription; }
        set { _itemDescription = value; }
    }

    public Sprite ThisItemSprite
    {
        get { return _itemSprite; }
        set { _itemSprite = value; }
    }
}
