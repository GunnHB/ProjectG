using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using UnityEditor.Animations;

[CreateAssetMenu(menuName = "Custom Create/Player Anim Ctrl Asset")]
public class PlayerAnimCtrlScriptableObject : SerializedScriptableObject
{
    [Tooltip("WeaponBase.WeaponType 의 순서와 동일하게 넣으시오")]
    public Dictionary<WeaponType, AnimatorController> _playerAnimCtrlDic;
}
