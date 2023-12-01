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
    [BoxGroup("Gravity"), SerializeField]
    protected float _maxDistance;
    [BoxGroup("Gravity"), SerializeField]
    protected LayerMask _groundMask;

    [BoxGroup("Gravity"), SerializeField, Space]
    protected bool _drawGizmos;

    private Vector3 _velocity;
    private bool _isGrounded = false;

    public Item.Data ItemData => _itemData;

    public bool IsGrounded
    {
        get => Physics.Raycast(transform.position, Vector3.down, _maxDistance);
    }

    protected virtual void Awake()
    {
        _collider = this.GetComponent<Collider>();
    }

    public void SetItemData(Item.Data newData)
    {
        _itemData = newData;
    }

    // // 중력 적용
    // private void FixedUpdate()
    // {
    //     if (!_isGrounded)
    //         ApplyGravity();
    // }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (GameManager.Instance.PController == null)
                return;

            UIManager.Instance.ShowItemInteractUI(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (GameManager.Instance.PController == null)
                return;

            UIManager.Instance.CloseItemInteractUI(this);
        }
    }

    private void ApplyGravity()
    {
        _velocity.y += GameValue.GRAVITY * Time.deltaTime;

        float posX = transform.position.x;
        float posY = transform.position.y + (_velocity.y * Time.deltaTime);
        float posZ = transform.position.z;

        transform.position = new Vector3(posX, posY, posZ);
    }

    private void OnDrawGizmos()
    {
        var isHit = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _maxDistance);

        Gizmos.color = Color.red;

        if (isHit)
            Gizmos.DrawRay(transform.position, Vector3.down * hit.distance);
        else
            Gizmos.DrawRay(transform.position, Vector3.down * _maxDistance);
    }
}
