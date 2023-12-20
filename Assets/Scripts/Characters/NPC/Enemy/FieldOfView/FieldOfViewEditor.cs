using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : OdinEditor
{
    private void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;

        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.ViewRadius);
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.MeleeAttackRange);

        Vector3 viewAngleA = fov.DirectionFromAngle(-fov.ViewAngle / 2, false);
        Vector3 viewAngleB = fov.DirectionFromAngle(fov.ViewAngle / 2, false);

        // // 삼각함수를 이용한 좌표 구하기
        // float posX = Mathf.Sin(fov.ViewAngle / 2 * Mathf.Deg2Rad) * fov.ViewRadius;
        // float posZ = Mathf.Cos(fov.ViewAngle / 2 * Mathf.Deg2Rad) * fov.ViewRadius;

        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.ViewRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.ViewRadius);

        Handles.color = Color.red;

        foreach (Transform visibleTarget in fov.VisibleTargetList)
            Handles.DrawLine(fov.transform.position, visibleTarget.position);
    }
}
