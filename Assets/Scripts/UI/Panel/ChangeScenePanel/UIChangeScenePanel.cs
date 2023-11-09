using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIChangeScenePanel : UIPanelBase
{
    [SerializeField]
    private Image _fadeImage;
    public Image FadeImage => _fadeImage;

    protected override void Awake()
    {
        base.Awake();
    }

    // 페이드 인 / 아웃
    public void SetImageAlpha(float alpha)
    {
        _fadeImage.color = new Color(0, 0, 0, alpha);
    }
}
