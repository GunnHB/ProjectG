using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum SceneType
{
    Title,
    InGame,
}

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        InitSingletonObjects();
    }

    private void InitSingletonObjects()
    {
        InitSingletonObject<ResourceManager>();
        InitSingletonObject<SceneManager>();
        InitSingletonObject<JsonManager>();
        InitSingletonObject<UIManager>();
        InitSingletonObject<EnemyManager>();
    }

    /// <summary>
    /// 매니저를 생성하기 위한 매서드
    /// (SingletonObject 상속받은 스크립트)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private void InitSingletonObject<T>()
    {
        GameObject prefab = new GameObject(typeof(T).Name);

        if (prefab != null)
            prefab.AddComponent(typeof(T));
    }
}
