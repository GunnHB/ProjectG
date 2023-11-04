using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class UIManager : SingletonObject<UIManager>
{
    private Canvas _popupCanvas;
    private Canvas _hudCanvas;

    public Canvas PrefabCanvas
    {
        get
        {
            if (_popupCanvas == null)
            {
                var canvasObject = GameObject.Find("PrefabCanvas");

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

    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 경로 값으로 프리팹 호출
    /// (Prefab/UI/ 이후의 값 입력)
    /// </summary>
    /// <param name="path"></param>
    /// <typeparam name="UIBase"></typeparam>
    public T OpenUI<T>(string path) where T : UIBase
    {
        T prefab = Resources.Load<T>($"Prefabs/UI/{path}");

        if (prefab == null)
        {
            Debug.LogWarning("There is no prefab! Please check the path!");
            return null;
        }

        T instanceObj = Instantiate(prefab, PrefabCanvas.transform);

        if (instanceObj == null)
            return null;

        // prefab 을 리턴하면 스크립트를 리턴 하는 것
        // 오브젝트를 리턴시킬 것
        return instanceObj;
    }

    public void OpenCommonDialogue(string title, string msg, UnityAction confirmAction, UnityAction cancelAction)
    {
        UIDialogueBase messageBox = OpenUI<UIDialogueBase>("Base/UIDialogueBase");

        if (messageBox == null)
            return;

        messageBox.InitUIDialogue(title, msg, confirmAction, cancelAction == null ? () => { CloseUI<UIDialogueBase>(); } : cancelAction);
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
