using UnityEngine;

using System.Collections.Generic;

using Sirenix.OdinInspector;

public class PlayerSkinnedMesh : SerializedMonoBehaviour
{
    public const string SKINNED_MESH_HAIR = "HAIR";
    public const string SKINNED_MESH_HAIR_HALF = "HAIR_HALF";
    public const string SKINNED_MESH_BELT = "BELT";
    public const string SKINNED_MESH_BOOTS = "BOOTS";
    public const string SKINNED_MESH_CLOTHES = "CLOTHES";
    public const string SKINNED_MESH_CROWN = "CROWN";
    public const string SKINNED_MESH_GLOVES = "GLOVES";
    public const string SKINNED_MESH_HAT = "HAT";
    public const string SKINNED_MESH_HELMET = "HELMET";
    public const string SKINNED_MESH_PAULDRONS = "PAULDRONS";
    public const string SKINNED_MESH_SKIN = "SKIN";

    [SerializeField] private Dictionary<string, SkinnedMeshRenderer> _skinnedDic = new();

    public Dictionary<string, SkinnedMeshRenderer> PlayerSkinnedMeshDic => _skinnedDic;

    public void SetPlayerSkinnedMesh(string key, Mesh mesh)
    {
        if (!_skinnedDic.ContainsKey(key) || mesh.name == string.Empty)
            return;

        _skinnedDic[key].sharedMesh = mesh;
    }
}
