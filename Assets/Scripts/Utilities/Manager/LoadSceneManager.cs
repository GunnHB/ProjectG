using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using Sirenix.OdinInspector;

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

    private UIChangeScenePanel _scenePanel;

    private Coroutine _fadeInCoroutine;
    private Coroutine _fadeOutCoroutine;

    private SceneType _sceneType;
    private UnityAction _callback;

    private bool _playFadeIn = false;
    private bool _invokedCallback = false;

    private float _fadingRate = .05f;

    protected override void Awake()
    {
        base.Awake();

        LoadPanel();
    }

    private void LoadPanel()
    {
        _scenePanel = UIManager.Instance.FindOpendUI<UIChangeScenePanel>(UIManager.Instance.PanelCanvas);

        if (_scenePanel == null)
            _scenePanel = UIManager.Instance.OpenUI<UIChangeScenePanel>("ChangeScenePanel/UIChangeScenePanel");

        // 클릭 허용
        _scenePanel.FadeImage.raycastTarget = false;
        _scenePanel.SetImageAlpha(0f);
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

    // 페이드 인 / 아웃 사이에 실행될 콜백 실행
    public void FadeInOut(UnityAction callback = null, SceneType type = SceneType.None)
    {
        // 전환 패널이 없는 경우 대비
        if (_scenePanel == null)
            LoadPanel();

        // 페이드 시작 중 뒤 ui 클릭 막기
        _scenePanel.FadeImage.raycastTarget = true;

        ResetCoroutineData();

        _callback = callback;
        _sceneType = type;

        _fadeInCoroutine = StartCoroutine(nameof(Cor_FadeIn));
        _fadeOutCoroutine = StartCoroutine(nameof(Cor_FadeOut));
    }

    private void ResetCoroutineData()
    {
        if (_fadeInCoroutine != null)
        {
            StopCoroutine(nameof(Cor_FadeIn));
            _fadeInCoroutine = null;
        }

        if (_fadeOutCoroutine != null)
        {
            StopCoroutine(nameof(Cor_FadeOut));
            _fadeOutCoroutine = null;
        }
    }

    private IEnumerator Cor_FadeIn()
    {
        float alpha = 0f;
        _scenePanel.transform.SetAsLastSibling();

        _playFadeIn = true;

        while (alpha < 1f)
        {
            alpha += _fadingRate;
            _scenePanel.SetImageAlpha(alpha);

            yield return new WaitForSeconds(_fadingRate);
        }

        _playFadeIn = false;
    }

    private IEnumerator Cor_FadeOut()
    {
        // 페이드 인 끝날 때까지 대기
        yield return new WaitUntil(() => !_playFadeIn);
        float alpha = 1f;

        while (alpha > 0f)
        {
            if (!_invokedCallback)
            {
                if (_sceneType != SceneType.None)
                    LoadScene(_sceneType);

                _callback?.Invoke();

                _invokedCallback = true;
                _scenePanel.transform.SetAsLastSibling();
            }

            if (_scenePanel == null)
            {
                // ResetData();
                // yield break;
                LoadPanel();
            }

            alpha -= _fadingRate;
            _scenePanel.SetImageAlpha(alpha);

            yield return new WaitForSeconds(_fadingRate);
        }

        ResetData();
    }

    private void ResetData()
    {
        _playFadeIn = false;
        _invokedCallback = false;
        _callback = null;
        _sceneType = SceneType.None;

        // 페이드 종료 후 ui 클릭 허용
        _scenePanel.FadeImage.raycastTarget = false;
    }
}
