using UnityEngine;

using System.Linq;

using System.Collections;
using System.Collections.Generic;

using Sirenix.OdinInspector;
using UnityEditor;

public class ItemBaseV2 : MonoBehaviour
{
    private const string SPRITE_PATH = "Assets/Resources/UI/Sprites/Inventory/Item";

    [ValueDropdown(nameof(ItemList)), HideLabel]
    [OnValueChanged(nameof(SetItemData)), SerializeField]
    private string _item;

    [SerializeField]
    private ItemDatabase _data;

    private Dictionary<string, long> _itemDic = new();

    private List<string> ItemList
    {
        get
        {
            Item.Data.Load();

            _itemDic.Clear();

            foreach (var item in Item.Data.DataList)
                _itemDic.Add(item.prefab_name, item.id);

            return _itemDic.Keys.ToList();
        }
    }

    private void SetItemData()
    {
        if (string.IsNullOrEmpty(_item) || _itemDic == null || _itemDic.Count == 0)
            return;

        if (_itemDic.TryGetValue(_item, out long itemId))
        {
            var itemData = Item.Data.DataMap[itemId];

            _data.ThisItemName = itemData.name;
            _data.ThisItemDesc = itemData.desc;
            _data.ThisItemSprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{SPRITE_PATH}/{itemData.type}/{itemData.image}.png");
        }
    }
}