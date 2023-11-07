using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FadeManager : SingletonObject<FadeManager>
{
    private Image _fadeImage;

    protected override void Awake()
    {
        base.Awake();

        // _fadeImage = UIManager.Instance.PrefabCanvas
    }

    public void DoFade(UnityAction callback)
    {
        ActiveFade(callback);
        DeactiveFade();
    }

    // 알파 0 -> 1
    private void ActiveFade(UnityAction callback)
    {
        // 알파 값이 1이 되면 콜백 호출함
    }

    // 알파 1 -> 0
    private void DeactiveFade()
    {

    }
}
