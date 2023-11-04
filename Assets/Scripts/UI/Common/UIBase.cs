using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Sirenix.OdinInspector;

public class UIBase : MonoBehaviour
{
    // 그룹명 상수
    const string BASE_SETTINGS = "Base Settings";

    [BoxGroup(BASE_SETTINGS)]
    [Tooltip("배경 딤드 여부")]
    [SerializeField] protected bool _backgroundDim = false;
    [BoxGroup(BASE_SETTINGS)]
    [Tooltip("배경 클릭 시 UI 닫을지")]
    [SerializeField] protected bool _backgroundClose = false;

    protected virtual void Start()
    {
        Init();
    }

    private void Init()
    {
        Image dimImage = this.gameObject.AddComponent<Image>();

        if (dimImage == null)
            return;

        RectTransform imageRect = dimImage?.transform as RectTransform;

        if (imageRect == null)
            return;

        // 요러면 strech / strech
        imageRect.anchorMax = Vector2.one;
        imageRect.pivot = new Vector2(.5f, .5f);

        if (_backgroundDim)
            dimImage.color = new Color(0f, 0f, 0f, .6f);

        if (_backgroundClose)
        {
            Button button = this.gameObject.AddComponent<Button>();

            if (button != null)
            {
                button.transition = Selectable.Transition.None;
                Util.AddButtonListener(button, () => UIManager.Instance.CloseUI<UIBase>());
            }
        }
    }
}
