using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

public class ItemMaker : OdinEditorWindow
{
    [MenuItem("Tools/CustomEditors/ItemMaker")]
    private static void Open()
    {
        GetWindow<ItemMaker>();
    }

    [BoxGroup("Select ItemType"), HideLabel, EnumToggleButtons]
    [OnValueChanged(nameof(SetItemPrefabs))]
    public ItemType _itemType;

    [TableList]
    public List<TableClass> _tableList = new();

    private List<ItemArmorBase> _armorList = new();
    private List<ItemFoodBase> _foodList = new();
    private List<ItemWeaponBase> _weaponList = new();
    private List<ItemDefaultBase> _defaultList = new();

    protected override void Initialize()
    {
        base.Initialize();

        Item.Data.Load();
        Weapon.Data.Load();

        // 처음 init 했을 때 한번 호출해주기
        SetItemPrefabs();
    }

    // 내가 무릎을 꿇었던 건 추진력을 얻기 위함
    private void SetItemPrefabs()
    {
        switch (_itemType)
        {
            case ItemType.Armor:
                ActualSetItemPrefabs(ref _armorList);
                break;
            case ItemType.Weapon:
                ActualSetItemPrefabs(ref _weaponList);
                break;
            case ItemType.Food:
                ActualSetItemPrefabs(ref _foodList);
                break;
            case ItemType.Default:
                ActualSetItemPrefabs(ref _defaultList);
                break;
        }
    }

    private void ActualSetItemPrefabs<T>(ref List<T> itemList) where T : ItemBase
    {
        var itemPrefabList = Resources.LoadAll($"Prefabs/Item/{_itemType}");

        foreach (var prefab in itemPrefabList)
        {
            var goPrefab = (GameObject)prefab;

            if (goPrefab.TryGetComponent(out T item))
            {
                if (itemList.Find(x => x == item) == null)
                    itemList.Add(item);
            }
            else
            {
                goPrefab.AddComponent<T>();

                // 모양에 맞지 않는 콜라이더가 붙을 수 있음둥
                // 어차피 크기 수정해야되니까 그 때 맞는 놈으로 다시 느시오
                if (!goPrefab.TryGetComponent(out CapsuleCollider collider))
                    goPrefab.AddComponent<CapsuleCollider>();

                if (goPrefab.TryGetComponent(out T compo))
                    itemList.Add(compo);
            }
        }

        SetTableList(itemList);
    }

    private void SetTableList<T>(List<T> itemList) where T : ItemBase
    {
        _tableList.Clear();

        // 할 게 없으요
        if (itemList.Count == 0)
            return;

        foreach (var item in itemList)
        {
            // var itemData = item.GetComponent<T>().ItemData;

            // if (itemData == null)
            //     itemData = new Item.Data();

            if (item.ThisItemData == null)
                item.SetItemData(new Item.Data());
            else
            {
                if (item.ThisItemData.Data == null)
                    item.ThisItemData.SetData(new Item.Data());
            }

            var itemData = item.ThisItemData.Data;

            TableClass newData = new(item, itemData);

            _tableList.Add(newData);
            newData._setDataCallback = SaveCallback;
        }
    }

    private void SaveCallback(ItemBase itemBase, Item.Data itemData)
    {
        itemBase.SetItemData(itemData);
    }

    // 테이블용 클래스
    [Serializable]
    public class TableClass
    {
        [TableColumnWidth(60, Resizable = false)]
        [PreviewField(Alignment = ObjectFieldAlignment.Center)]
        public GameObject _prefab;
        // Item.Data 변수들
        // 테이블에 컬럼 추가되면 여기에도 추가합시다. (그래야 수정하고 싶을 때 보입니다.)
        [VerticalGroup("Id"), HideLabel]
        public long _id;
        public ItemType _type;
        public string _name;
        public string _desc;
        public string _image;
        public long _ref_id;
        public string _prefab_name;
        [TableColumnWidth(80, Resizable = false)]
        public bool _stackable;

        public UnityAction<ItemBase, Item.Data> _setDataCallback;

        private ItemBase _itemBase;
        private Item.Data _itemData;

        public TableClass(ItemBase itemBase, Item.Data itemData)
        {
            this._prefab = itemBase.gameObject;

            SetItemDataValue(itemData);

            this._itemBase = itemBase;
            this._itemData = itemData;
        }

        private void SetItemDataValue(Item.Data data)
        {
            this._id = data.id;
            this._type = data.type;
            this._name = data.name;
            this._desc = data.desc;
            this._image = data.image;
            this._ref_id = data.ref_id;
            this._stackable = data.stackable;
            this._prefab_name = data.prefab_name;
        }

        [VerticalGroup("Id")]
        [Button]
        public void SetData()
        {
            var data = Item.Data.GetList().Find(x => x.id == _id);

            SetItemDataValue(data);

            _itemData = data;
            _prefab_name = _itemBase.name;

            EditorUtility.SetDirty(this._itemBase.gameObject);
        }

        [VerticalGroup("Actions")]
        [Button]
        public void ResetData()
        {
            // this._itemData = this._itemBase.ItemData;
            this._itemData = this._itemBase.ThisItemData.Data;

            SetItemDataValue(_itemData);
        }

        [VerticalGroup("Actions")]
        [Button]
        public void SaveData()
        {
            var itemData = new Item.Data
            {
                id = this._id,
                type = this._type,
                name = this._name,
                desc = this._desc,
                image = this._image,
                ref_id = this._ref_id,
                stackable = this._stackable,
                prefab_name = this._prefab_name,
            };

            Item.Data.Write(itemData);

            _setDataCallback?.Invoke(_itemBase, itemData);

            EditorUtility.SetDirty(_itemBase);
        }
    }
}
