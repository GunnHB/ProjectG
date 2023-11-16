using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPool
{
    public GameObject _poolPrefab;
    public GameObject _parentObj;

    private Queue<GameObject> _poolQueue = new Queue<GameObject>();
    public Queue<GameObject> PoolQueue => _poolQueue;

    public void Initialize(GameObject parentObj = null)
    {
        if (parentObj != null)
            _parentObj = parentObj;
    }

    public GameObject CreateNewObject()
    {
        GameObject tempObj = GameObject.Instantiate(_poolPrefab);

        tempObj.SetActive(false);

        if (_parentObj != null)
            tempObj.transform.SetParent(_parentObj.transform);

        return tempObj;
    }

    public GameObject GetObject(Vector3 position, Quaternion quaternion)
    {
        if (_poolQueue.Count > 0)
        {
            GameObject tempObj = _poolQueue.Dequeue();

            tempObj.transform.position = position;
            tempObj.transform.eulerAngles = quaternion.eulerAngles;

            tempObj.SetActive(true);

            return tempObj;
        }
        else
        {
            GameObject newObj = CreateNewObject();

            newObj.transform.position = position;
            newObj.transform.eulerAngles = quaternion.eulerAngles;

            newObj.SetActive(true);

            return newObj;
        }
    }

    public GameObject GetObject()
    {
        if (_poolQueue.Count > 0)
        {
            GameObject temp = _poolQueue.Dequeue();
            temp.SetActive(true);

            return temp;
        }
        else
        {
            var temp = CreateNewObject();
            temp.SetActive(true);

            return temp;
        }
    }

    public void ReturnObject(GameObject obj)
    {
        _poolQueue.Enqueue(obj);

        if (_parentObj != null)
            obj.transform.SetParent(_parentObj.transform);

        obj.SetActive(false);
    }

    public void ReturnAllObject()
    {
        for (int index = 0; index < _parentObj.transform.childCount; index++)
        {
            var item = _parentObj.transform.GetChild(index);

            // 켜진 애들만 리턴
            if (item.gameObject.activeInHierarchy)
                ReturnObject(item.gameObject);
        }
    }
}
