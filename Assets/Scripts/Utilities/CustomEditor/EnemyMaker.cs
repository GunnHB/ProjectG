using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

using Sirenix.OdinInspector.Editor;

public class EnemyData
{
    public long _id;
    public string _name;
}

public class EnemyMaker : OdinMenuEditorWindow
{
    [MenuItem("Tools/CustomEditors/EnemyMaker")]
    private static void Open()
    {
        GetWindow<EnemyMaker>();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        throw new System.NotImplementedException();
        var tree = new OdinMenuTree();
    }
}
