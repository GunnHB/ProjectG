using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

using UnityEditor;

public class ItemSOMaker : OdinEditorWindow
{
    [MenuItem("Tools/CustomEditors/ItemSOMaker")]
    private static void Open()
    {
        GetWindow<ItemSOMaker>();
    }

    [InlineEditor]
    public GameObject _itemPrefab;

    protected override void Initialize()
    {
        base.Initialize();

        // 테이블 데이터 로드
        Item.Data.Load();
        Weapon.Data.Load();
    }
}

public class ItemScriptableObject : SerializedScriptableObject
{
    public GameObject _prefab;
    public Item.Data _itemData;
}

public class WeaponScriptableObject : ItemScriptableObject
{
    public Weapon.Data _weaponData;
}