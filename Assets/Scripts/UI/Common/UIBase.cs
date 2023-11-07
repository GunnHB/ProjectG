using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;

using Sirenix.OdinInspector;

public class UIBase : MonoBehaviour
{
    public enum UIBaseType
    {
        None,
        Popup,
        Dialogue,
        Toast,
        HUD,
        Panel,
    }

    // 그룹명 상수
    const string BASE_SETTINGS = "Base Settings";

    [BoxGroup(BASE_SETTINGS)]
    [Tooltip("배경 딤드 여부")]
    [SerializeField] protected bool _backgroundDim = false;
    [BoxGroup(BASE_SETTINGS)]
    [Tooltip("배경 클릭 시 UI 닫을지")]
    [SerializeField] protected bool _backgroundClose = false;

    protected UIBaseType _uiType;

    public UIBaseType UIType => _uiType;

    protected virtual void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (!_backgroundDim && !_backgroundClose)
            return;

        var optionObj = new GameObject("OptionalArea");

        optionObj.transform.parent = this.gameObject.transform;
        optionObj.transform.SetSiblingIndex(0);

        if (_backgroundDim)
        {
            var dimImage = optionObj.AddComponent<Image>();

            if (dimImage == null)
                return;

            var optionRect = optionObj.transform as RectTransform;

            optionRect.anchoredPosition = Vector2.zero;

            // 요러면 strech / strech
            optionRect.anchorMin = Vector2.zero;
            optionRect.anchorMax = Vector2.one;
            optionRect.pivot = new Vector2(.5f, .5f);

            dimImage.color = new Color(0f, 0f, 0f, .6f);
        }

        if (_backgroundClose)
        {
            var closeButton = optionObj.AddComponent<Button>();

            if (closeButton == null)
                return;

            closeButton.transition = Selectable.Transition.None;
            Util.AddButtonListener(closeButton, () => UIManager.Instance.CloseUI<UIBase>());
        }
    }

    public virtual void Open()
    {

    }

    public virtual void Close()
    {
        UIManager.Instance.CloseUI<UIBase>();
    }
}
