using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UnityEditor;
using UnityEditor.UI;

using Sirenix.Utilities.Editor;
using Sirenix.Utilities;

[CustomEditor(typeof(CommonButton))]
public class CommonButtonEditor : ButtonEditor
{
    public override void OnInspectorGUI()
    {
        CommonButton _commonButton = (CommonButton)target;

        bool curPressToggle = _commonButton._needPress;
        bool curEnterToggle = _commonButton._needEnterAndExit;

        // 그룹핑
        SirenixEditorGUI.BeginBox("[Optional]", true, null);
        _commonButton._needPress = EditorGUILayout.Toggle("Check press", _commonButton._needPress);
        _commonButton._needEnterAndExit = EditorGUILayout.Toggle("Check enter and exit", _commonButton._needEnterAndExit);
        SirenixEditorGUI.EndBox();

        base.OnInspectorGUI();

        // 변경 사항 저장
        if (_commonButton._needPress != curPressToggle || _commonButton._needEnterAndExit != curEnterToggle)
            EditorUtility.SetDirty(target);
    }
}
