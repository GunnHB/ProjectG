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

    public Item.Data ItemData => _itemData;

    protected virtual void Awake()
    {
        _collider = this.GetComponent<Collider>();
    }

    public void SetItemData(Item.Data newData)
    {
        _itemData = newData;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (GameManager.Instance.PController == null)
                return;

            GameManager.Instance.PController._itemEnterAction?.Invoke(this);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (GameManager.Instance.PController == null)
                return;

            GameManager.Instance.PController._itemExitAction?.Invoke(this);
        }
    }
}
