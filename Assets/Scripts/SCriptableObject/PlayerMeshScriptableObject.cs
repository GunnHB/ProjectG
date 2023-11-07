using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Custom Create/Player Mesh Asset")]
public class PlayerMeshScriptableObject : SerializedScriptableObject
{
    [Title("[Hair]")]
    public Mesh[] _hairMeshArray;
    public Mesh[] _halfHairMeshArray;

    [Title("[Head]")]
    public Mesh[] _helmetMeshArray;
    public Mesh[] _crownMeshArray;
    public Mesh[] _hatMeshArray;

    [Title("[Skin]")]
    public Mesh[] _skinMeshArray;

    [Title("[Armor]")]
    public Mesh[] _clothesMeshArray;
    public Mesh[] _beltMeshArray;
    public Mesh[] _glovesMeshArray;
    public Mesh[] _pauldronMeshArray;
    public Mesh[] _bootMeshArray;
}
