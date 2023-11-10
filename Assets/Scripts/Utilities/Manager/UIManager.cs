using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public partial class UIManager : SingletonObject<UIManager>
{
    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 경로 값으로 프리팹 호출
    /// (Prefabs/UI/{builder}/ 가 기본)
    /// </summary>
    public T OpenUI<T>(string path) where T : UIBase
    {
        StringBuilder builder = new StringBuilder();

        Transform parent = GetParentCanvasTransform<T>();        // 부모 캔버스 프랜스폼

        builder.Append(GetParentCanvasPath<T>());
        builder.Append(path);

        T prefab = Resources.Load<T>($"Prefabs/UI/{builder}");

        if (prefab == null)
        {
            Debug.LogWarning("There is no prefab! Please check the path!");
            return null;
        }

        T instanceObj = Instantiate(prefab, parent);

        if (instanceObj == null)
            return null;

        // prefab 을 리턴하면 스크립트를 리턴 하는 것
        // 오브젝트를 리턴시킬 것
        return instanceObj;
    }

    // 해당하는 ui 있으면 반환 (가장 상단에 있는걸로)
    public T FindOpendUI<T>(Canvas canvas) where T : UIBase
    {
        T findValue = null;

        for (int index = 0; index < canvas.transform.childCount; index++)
        {
            var item = canvas.transform.GetChild(index);

            if (item.TryGetComponent(out T compo))
            {
                findValue = compo;
                break;
            }
        }

        return findValue;
    }

    public T FindOpendUI<T>() where T : UIBase
    {
        T findUI = null;
        Transform parentCanvas = GetParentCanvasTransform<T>();

        for (int index = 0; index < parentCanvas.childCount; index++)
        {
            var item = parentCanvas.GetChild(index);

            if (item.TryGetComponent(out T compo))
            {
                findUI = compo;
                break;
            }
        }

        return findUI;
    }

    // 나중에 로컬라이즈용 테이블 만들어지면 수정하자
    public void OpenSimpleDialogue(string title, string msg,
                                   string confirmString = "Confirm", UnityAction confirmAction = null,
                                   string cancelString = "Cancel", UnityAction cancelAction = null)
    {
        var messageBox = OpenUI<UISimpleDialogue>("SimpleDialogue/UISimpleDialogue");

        if (messageBox == null)
            return;

        messageBox.InitUIDialogue(title, msg,
                                  confirmString, confirmAction,
                                  cancelString, cancelAction);
    }

    private Transform GetParentCanvasTransform<T>()
    {
        if (typeof(T).BaseType.Equals(typeof(UIPanelBase)))
            return PanelCanvas.transform;
        else if (typeof(T).BaseType.Equals(typeof(UIHUDBase)))
            return HudCanvas.transform;
        else if (typeof(T).BaseType.Equals(typeof(UIPopupBase)))
            return PopupCanvas.transform;
        else if (typeof(T).BaseType.Equals(typeof(UIDialogueBase)))
            return DialogueCanvas.transform;

        return null;
    }

    private string GetParentCanvasPath<T>()
    {
        if (typeof(T).BaseType.Equals(typeof(UIPanelBase)))
            return PANEL;
        else if (typeof(T).BaseType.Equals(typeof(UIHUDBase)))
            return HUD;
        else if (typeof(T).BaseType.Equals(typeof(UIPopupBase)))
            return POPUP;
        else if (typeof(T).BaseType.Equals(typeof(UIDialogueBase)))
            return DIALOGUE;

        return string.Empty;
    }

    public void CloseUI(Transform uiTransform)
    {
        // GameObject prefab = GetParentCanvasTransform<T>().GetComponentInChildren<T>().gameObject;
        GameObject prefab = uiTransform.gameObject;

        if (prefab != null)
            Destroy(prefab);
    }
}
