using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public partial class UIManager : SingletonObject<UIManager>
{
    private const string POPUP = "Popup/";
    private const string PANEL = "Panel/";
    private const string HUD = "HUD/";
    private const string DIALOGUE = "Dialogue/";
    private const string TOAST = "Toast/";
    private const string INTERACT = "Interact/";

    private Canvas _popupCanvas;
    private Canvas _hudCanvas;
    private Canvas _panelCanvas;
    private Canvas _dialogueCanvas;
    private Canvas _toastCanvas;
    private Canvas _interactCanvas;

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
                var canvasObject = GameObject.Find("DialogueCanvas");

                if (canvasObject != null)
                    _dialogueCanvas = canvasObject.GetComponent<Canvas>();
            }

            return _dialogueCanvas;
        }
    }

    public Canvas ToastCanvas
    {
        get
        {
            if (_toastCanvas == null)
            {
                var canvasObject = GameObject.Find("ToastCanvas");

                if (canvasObject != null)
                    _toastCanvas = canvasObject.GetComponent<Canvas>();
            }

            return _toastCanvas;
        }
    }

    public Canvas InteractCanvas
    {
        get
        {
            if (_interactCanvas == null)
            {
                var interactObj = GameObject.Find("InteractCanvas");

                if (interactObj != null)
                    _interactCanvas = interactObj.GetComponent<Canvas>();
            }

            return _interactCanvas;
        }
    }
}
