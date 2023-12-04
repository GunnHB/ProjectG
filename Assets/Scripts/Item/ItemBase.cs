using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class ItemData
{
    [SerializeField]
    private Item.Data _data = null;
    [SerializeField]
    private bool _isEquip = false;
    [SerializeField]
    private int _amount = 0;

    public Item.Data Data => _data;
    public bool IsEquip => _isEquip;
    public int Amount => _amount;

    public bool IsEmpty
    {
        get => this._data == null || this._data.id == 0;
    }

    public ItemData()
    {
        this._data = new Item.Data();
        this._isEquip = false;
        this._amount = 1;
    }

    public ItemData(Item.Data data, bool isEquip = false, int amount = 1)
    {
        this._data = data;
        this._isEquip = isEquip;
        this._amount = amount;
    }

    public void ResetData()
    {
        this._data = new Item.Data();
        this._isEquip = false;
        this._amount = 0;
    }

    public void SetData(Item.Data data)
    {
        this._data = data;
    }

    public void SetEquip(bool active)
    {
        this._isEquip = active;
    }

    // 원하는 수량으로 세팅
    public void SetAmount(int amount)
    {
        this._amount = amount;
    }

    // 수량 1 증가 or 감소
    public void AddItemAmount(bool isPlus = true)
    {
        if (isPlus)
            _amount++;
        else
            _amount--;
    }

    public void SetItemData(ItemData itemData)
    {
        SetData(itemData.Data);
        SetEquip(itemData.IsEquip);
        SetAmount(itemData.Amount);
    }
}

public class ItemBase : SerializedMonoBehaviour
{
    [SerializeField]
    // protected Item.Data _itemData;
    protected ItemData _itemData;
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

    // public Item.Data ItemData => _itemData;
    public ItemData ThisItemData => _itemData;
    public bool IsGrounded => Physics.Raycast(transform.position, Vector3.down, _maxDistance);

    protected virtual void Awake()
    {
        _collider = this.GetComponent<Collider>();
    }

    public void SetItemData(Item.Data newData)
    {
        // _itemData = newData;
        // _itemData.SetData(newData);

        if (_itemData == null)
            _itemData = new ItemData();

        _itemData.SetData(newData);
    }

    // // 중력 적용
    // private void FixedUpdate()
    // {
    //     if (!_isGrounded)
    //         ApplyGravity();
    // }

    private void OnTriggerEnter(Collider other)
    {
        if (this._itemData.IsEquip)
            return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (GameManager.Instance.PController == null)
                return;

            UIManager.Instance.ShowItemInteractUI(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (this._itemData.IsEquip)
            return;

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
