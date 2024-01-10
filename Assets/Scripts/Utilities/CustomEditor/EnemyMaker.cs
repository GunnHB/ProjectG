using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

using Sirenix.OdinInspector.Editor;

public class EnemyMaker : OdinMenuEditorWindow
{
    private const string PATH = "Assets/Resources/Prefabs/Characters/Enemies";

    [MenuItem("Tools/CustomEditors/EnemyMaker")]
    private static void Open()
    {
        GetWindow<EnemyMaker>();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        // throw new System.NotImplementedException();
        var tree = new OdinMenuTree();

        tree.AddAllAssetsAtPath("EnemyDataBase", PATH, typeof(EnemyBase));

        return tree;
    }
}
