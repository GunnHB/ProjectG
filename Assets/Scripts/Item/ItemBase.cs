using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : MonoBehaviour
{
    private Item.Data _itemData;

    private Collider _collider;         // 충돌 감지용
    private Rigidbody _righdBody;       // 중력 적용

    public Item.Data ItemData => _itemData;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (GameManager.Instance.PController == null)
                return;

            GameManager.Instance.PController._itemEnterAction?.Invoke(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (GameManager.Instance.PController == null)
                return;

            GameManager.Instance.PController._itemExitAction?.Invoke(this);
        }
    }

    protected virtual void PickUpItem()
    {

    }

    protected virtual void DestoryItem()
    {

    }
}
