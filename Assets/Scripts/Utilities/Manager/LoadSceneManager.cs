using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : SingletonObject<LoadSceneManager>
{
    public const string START_SCENE = "StartScene";
    public const string IN_GAME_SCENE = "InGameScene";

    public enum SceneType
    {
        None,
        Start,
        InGame,
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public void LoadScene(SceneType type)
    {
        SceneManager.LoadScene(SetLoadScene(type));
    }

    private string SetLoadScene(SceneType type)
    {
        switch (type)
        {
            case SceneType.Start:
                return START_SCENE;
            case SceneType.InGame:
                return IN_GAME_SCENE;
        }

        return string.Empty;
    }
}
