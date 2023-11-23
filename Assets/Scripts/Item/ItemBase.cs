using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class ItemBase : SerializedMonoBehaviour
{
    [SerializeField]
    protected Item.Data _itemData;

    [SerializeField]
    protected Collider _collider;         // 충돌 감지용
    [SerializeField]
    protected Rigidbody _rigidBody;       // 중력 적용

    public Item.Data ItemData => _itemData;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("여기는...???");
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("여기는 탐1");
            if (GameManager.Instance.PController == null)
                return;

            GameManager.Instance.PController._itemEnterAction?.Invoke(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("여기는 탐2");
            if (GameManager.Instance.PController == null)
                return;

            GameManager.Instance.PController._itemExitAction?.Invoke(this);
        }
    }
}
