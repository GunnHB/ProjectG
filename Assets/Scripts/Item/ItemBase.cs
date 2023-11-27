using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class ItemBase : SerializedMonoBehaviour
{
    [SerializeField]
    protected Item.Data _itemData;
    protected Collider _collider;         // 충돌 감지용

    [BoxGroup("Gravity"), SerializeField]
    protected Vector3 _boxSize;
    [BoxGroup("Gravity"), SerializeField, Space]
    protected float _maxDistance;

    [BoxGroup("Gravity"), SerializeField]
    protected bool _drawGizmos;

    private Vector3 _velocity;
    private bool _isGrounded = false;

    public Item.Data ItemData => _itemData;

    protected virtual void Awake()
    {
        _collider = this.GetComponent<Collider>();
    }

    public void SetItemData(Item.Data newData)
    {
        _itemData = newData;
    }

    // 중력 적용
    private void FixedUpdate()
    {
        if (!_isGrounded)
            ApplyGravity();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (GameManager.Instance.PController == null)
                return;

            Debug.Log("ui 켜기!");
            // GameManager.Instance.PController._itemEnterAction?.Invoke(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (GameManager.Instance.PController == null)
                return;

            Debug.Log("ui 끄기!");
            // GameManager.Instance.PController._itemEnterAction?.Invoke(this);
        }
    }

    private void ApplyGravity()
    {
        _velocity.y += GameValue.GRAVITY * Time.deltaTime;
        transform.position += _velocity * Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        var boolean = Physics.BoxCast(transform.position, _boxSize, -Vector3.up, out RaycastHit hit,
                                      transform.rotation, _maxDistance, LayerMask.NameToLayer("Ground"));

        if (boolean)
        {
            if (hit.distance <= _maxDistance)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(transform.position - Vector3.up * hit.distance, _boxSize);
            }
            else
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(transform.position - Vector3.up * _maxDistance, _boxSize);
            }
        }
        else
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(transform.position - Vector3.up * _maxDistance, _boxSize);
        }
    }
}
