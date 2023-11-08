using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : SingletonObject<EnemyManager>
{
    [SerializeField] private List<ObjectPool> _enemyObjectList = new();

    protected override void Awake()
    {
        base.Awake();

        // Init();
    }

    private void Init()
    {
        GetAllEnemy();

        foreach (var enemy in _enemyObjectList)
            enemy.Initialize();
    }

    private void GetAllEnemy()
    {
        var objectList = Util.GetAllObject("Prefabs/Characters/Enemies");

        foreach (var obj in objectList)
        {
            var tempPool = new ObjectPool()
            {
                _amount = 1,
                _poolPrefab = obj,
            };

            _enemyObjectList.Add(tempPool);
        }
    }
}
