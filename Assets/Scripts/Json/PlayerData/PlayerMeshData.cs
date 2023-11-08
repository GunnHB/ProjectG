using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class PlayerMeshData
{
    public Mesh[] _hairHesh;
    public Mesh[] _skinMesh;

    public PlayerMeshData()
    {
        _hairHesh = new Mesh[GameValue.SAVE_SLOT_COUNT];
        _skinMesh = new Mesh[GameValue.SAVE_SLOT_COUNT];
    }
}
