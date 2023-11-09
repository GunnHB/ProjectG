using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : SingletonObject<UIManager>
{
    private const string POPUP = "Popup/";
    private const string PANEL = "Panel/";
    private const string HUD = "HUD/";

    private Canvas _popupCanvas;
    private Canvas _hudCanvas;
    private Canvas _panelCanvas;
    private Canvas _dialogueCanvas;

    public Canvas PopupCanvas
    {
        get
        {
            if (_popupCanvas == null)
            {
                var canvasObject = GameObject.Find("PopupCanvas");

                if (canvasObject != null)
                    _popupCanvas = canvasObject.GetComponent<Canvas>();
            }

            return _popupCanvas;
        }
    }
    public Canvas HudCanvas
    {
        get
        {
            if (_hudCanvas == null)
            {
                var canvasObject = GameObject.Find("HUDCanvas");

                if (canvasObject != null)
                    _hudCanvas = canvasObject.GetComponent<Canvas>();
            }

            return _hudCanvas;
        }
    }

    public Canvas PanelCanvas
    {
        get
        {
            if (_panelCanvas == null)
            {
                var canvasObject = GameObject.Find("PanelCanvas");

                if (canvasObject != null)
                    _panelCanvas = canvasObject.GetComponent<Canvas>();
            }

            return _panelCanvas;
        }
    }

    public Canvas DialogueCanvas
    {
        get
        {
            if (_dialogueCanvas == null)
            {
                var canvasObject = GameObject.Find("PanelCanvas");

                if (canvasObject != null)
                    _dialogueCanvas = canvasObject.GetComponent<Canvas>();
            }

            return _dialogueCanvas;
        }
    }


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
        Transform parent = null;

        if (typeof(T).BaseType.Equals(typeof(UIPanelBase)))
        {
            builder.Append(PANEL);
            parent = PanelCanvas.transform;
        }
        else if (typeof(T).BaseType.Equals(typeof(UIHUDBase)))
        {
            builder.Append(HUD);
            parent = HudCanvas.transform;
        }
        else if (typeof(T).BaseType.Equals(typeof(UIPopupBase)))
        {
            builder.Append(POPUP);
            parent = PopupCanvas.transform;
        }

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

    public void OpenCommonDialogue(string title, string msg,
                                   string confirmString = "Confirm", UnityAction confirmAction = null,
                                   string cancelString = "Cancel", UnityAction cancelAction = null)
    {
        UIDialogueBase messageBox = OpenUI<UIDialogueBase>("Base/UIDialogueBase");

        if (messageBox == null)
            return;

        messageBox.InitUIDialogue(title, msg,
                                  confirmString, confirmAction,
                                  cancelString, cancelAction);
    }

    public void CloseUI<T>() where T : UIBase
    {
        GameObject prefab = _popupCanvas.GetComponentInChildren<T>().gameObject;

        if (prefab != null)
            Destroy(prefab);
    }

    public void CloseDialogue()
    {
        CloseUI<UIDialogueBase>();
    }
}
