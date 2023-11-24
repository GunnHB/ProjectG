using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Unity.VisualScripting;
using System;

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

    // [TableList, ShowIf(nameof(_itemType), ItemType.Armor), SerializeField]
    // public TableClass _armorTable = new();

    // [TableList, ShowIf(nameof(_itemType), ItemType.Weapon), SerializeField]
    // public TableClass _weaponTable = new();

    // [TableList, ShowIf(nameof(_itemType), ItemType.Food)]
    // public TableClass _foodTable = new();

    // [TableList, ShowIf(nameof(_itemType), ItemType.Default)]
    // public TableClass _defaultTable = new();

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

    // 내가 무릎을 굽힌건 추진력을 얻기 위함
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
        Debug.Log(_itemType);

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
            var itemData = item.GetComponent<T>().ItemData;

            _tableList.Add(new(item.gameObject, itemData == null ? new Item.Data() : itemData));
        }
    }

    [Button("Save item data")]
    public void SaveItemData()
    {
        Debug.Log("SAVE!");
    }

    // 테이블용 클래스
    public class TableClass
    {
        public GameObject _prefab;

        public TableClass(GameObject prefab, Item.Data data)
        {
            this._prefab = prefab;
            Activator.CreateInstance(data.GetType());

            foreach (var field in data.GetType().GetFields(System.Reflection.BindingFlags.Public |
                                                           System.Reflection.BindingFlags.Instance))
            {
                Debug.Log(field.Name);
            }
        }
    }
}
