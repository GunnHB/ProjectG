using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ModelItem
{
    public long _id;
    public ItemType _itemType;
    public string _itemName;
    public string _itemDescription;
    public string _itemImage;
    public long _refId;

    public ModelItem()
    {
        _id = 0;
        _itemType = ItemType.Armor;
        _itemName = string.Empty;
        _itemDescription = string.Empty;
        _itemImage = string.Empty;
        _refId = 0;
    }
}

public enum ItemType
{
    Armor,
    Weapon,
    Food,
    Default,
}
