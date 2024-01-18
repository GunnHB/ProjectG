using UnityEngine;

using System.Linq;

using System.Collections.Generic;

using Sirenix.OdinInspector;

public class ItemBaseV2 : MonoBehaviour
{
    private const string SPRITE_PATH = "Assets/Resources/UI/Sprites/Inventory/Item";

    [ValueDropdown(nameof(ItemList)), HideLabel]
    [OnValueChanged(nameof(SetItemId)), SerializeField]
    protected string _item;

    [SerializeField]
    protected long _itemId;

    [SerializeField]
    private Collider _collider;

    public Collider ThisCollider => _collider;

    private Dictionary<string, long> _itemDic = new();

    public long ThisItemId => _itemId;

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

    private void SetItemId()
    {
        if (string.IsNullOrEmpty(_item))
            return;

        if (_itemDic.TryGetValue(_item, out long itemId))
            _itemId = itemId;
    }

    // 플레이어와의 충돌을 감지
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.TryGetComponent(out PlayerController player))
            Debug.Log("show");
    }

    // 플레이어와의 충돌을 감지
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.TryGetComponent(out PlayerController player))
            Debug.Log("hide");
    }
}