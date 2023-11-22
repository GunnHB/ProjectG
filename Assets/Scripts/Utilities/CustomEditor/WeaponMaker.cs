using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

using UnityEditor;
using System;

public class WeaponMaker : OdinEditorWindow
{
    private const string WEAPON_GROUP = "WeaponGroup";

    [MenuItem("Tools/CustomEditors/WeaponMaker")]
    private static void Open()
    {
        GetWindow<WeaponMaker>();
    }

    [TableList(AlwaysExpanded = true)]
    public List<Weapon.Data> _weaponTableList = new();

    protected override void Initialize()
    {
        base.Initialize();
        Weapon.Data.Load();

        _weaponTableList.AddRange(Weapon.Data.DataList);
    }
}
