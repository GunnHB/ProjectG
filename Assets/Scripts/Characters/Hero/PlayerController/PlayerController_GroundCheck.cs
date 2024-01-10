using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public partial class PlayerController : CharacterBase
{
    [Title("[Boxcast properties]")]
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private Vector3 _boxSize;
    [SerializeField] private float _maxDistance;
    [SerializeField] private LayerMask _groundMask;

    [Title("[Debug]")]
    [SerializeField] private bool _drawGizmo;

    public bool IsGrounded
    {
        get => Physics.BoxCast(_targetTransform.position, _boxSize, -transform.up, transform.rotation, _maxDistance, _groundMask);
    }

    // ground check gizmo
    private void OnDrawGizmos()
    {
        if (!_drawGizmo)
            return;

        Gizmos.color = IsGrounded ? Color.red : Color.blue;
        Gizmos.DrawCube(_targetTransform.position - transform.up * _maxDistance, _boxSize);
    }
}
