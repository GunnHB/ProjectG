using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using UnityEngine.EventSystems;
using Unity.VisualScripting;

public static class Util
{
    /// <summary>
    /// 기본 버튼 리스너
    /// </summary>
    /// <param name="button"></param>
    /// <param name="callback"></param>
    public static void AddButtonListener(Button button, UnityAction callback)
    {
        if (button == null)
            return;

        // 공용 버튼인지 확인
        if (!(button as CommonButton))
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(callback);
        }
        else
        {
            var commonButton = button as CommonButton;

            // 이벤트 트리거 처리 필요한지
            if (commonButton._needPress || commonButton._needEnterAndExit)
            {
                if (commonButton._needPress)
                {
                    AddButtonListener(button, EventTriggerType.PointerDown, callback);
                    AddButtonListener(button, EventTriggerType.PointerUp, callback);
                }

                if (commonButton._needEnterAndExit)
                {
                    AddButtonListener(button, EventTriggerType.PointerEnter, callback);
                    AddButtonListener(button, EventTriggerType.PointerExit, callback);
                }
            }
            // 처리 필요 없으면 일반적인 리스너 달아주기
            else
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(callback);
            }
        }
    }

    // 일반 클릭 콜백
    public static void AddButtonListenerV2(Button button, UnityAction onClickCallback, bool removeAll = true)
    {
        if (button == null)
            return;

        if (removeAll)
            button.onClick.RemoveAllListeners();

        button.onClick.AddListener(onClickCallback);
    }

    // 버튼 press 여부 콜백
    public static void AddPressButtonListener(Button button,
                                              UnityAction downCallback, UnityAction upCallback = null, UnityAction clickCallback = null)
    {
        if (button == null)
            return;

        if (clickCallback != null)
            AddButtonListenerV2(button, clickCallback);

        // 상태 변수 확인을 위해 파라미터의 콜백을 한번 가공
        UnityAction downAction = () =>
        {
            if (!(button as CommonButton).IsPress)
                (button as CommonButton).SetPress(true);
        };

        UnityAction upAction = () =>
        {
            if ((button as CommonButton).IsPress)
                (button as CommonButton).SetPress(false);
        };

        downAction += downCallback;
        upAction += upCallback;

        AddButtonTrigger(button, EventTriggerType.PointerDown, downAction);
        AddButtonTrigger(button, EventTriggerType.PointerUp, upAction);
    }

    // 버튼 Hover 여부 콜백
    public static void AddHoverButtonListener(Button button,
                                              UnityAction enterCallback, UnityAction exitCallback = null, UnityAction clickCallback = null)
    {
        if (button == null)
            return;

        if (clickCallback != null)
            AddButtonListenerV2(button, clickCallback);

        // 상태 변수 확인을 위해 파라미터의 콜백을 한번 가공
        UnityAction enterAction = () =>
        {
            if (!(button as CommonButton).IsEnter)
                (button as CommonButton).SetEnter(true);
        };

        UnityAction exitAction = () =>
        {
            if ((button as CommonButton).IsEnter)
                (button as CommonButton).SetEnter(false);
        };

        enterAction += enterCallback;
        exitAction += exitCallback;

        AddButtonTrigger(button, EventTriggerType.PointerEnter, enterAction);
        AddButtonTrigger(button, EventTriggerType.PointerExit, exitAction);
    }

    // 특정 트리거 타입의 콜백을 모두 삭제
    public static void RemoveTrigger(Button button, EventTriggerType triggerType)
    {
        if (button == null)
            return;

        if (button.TryGetComponent(out EventTrigger trigger))
        {
            var item = trigger.triggers.Find(ev => ev.eventID == triggerType);

            if (item == null)
                return;

            item.callback.RemoveAllListeners();
        }
    }

    // 버튼에 달려 있는 모든 트리거 타입의 콜백 삭제
    public static void RemoveAllTriggers(Button button)
    {
        if (button == null)
            return;

        if (button.TryGetComponent(out EventTrigger trigger))
        {
            foreach (var item in trigger.triggers)
                item.callback.RemoveAllListeners();
        }
    }

    public static void AddButtonTrigger(Button button, EventTriggerType triggerType, UnityAction callback)
    {
        if (button == null)
            return;

        var trigger = button.GetComponent<EventTrigger>();

        if (trigger == null)
        {
            button.AddComponent<EventTrigger>();
            trigger = button.GetComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = triggerType;
        entry.callback.AddListener((eventData) => { callback?.Invoke(); });

        trigger.triggers.Add(entry);
    }

    /// <summary>
    /// EventTrigger 기능
    /// </summary>
    /// <param name="button"></param>
    /// <param name="callback"></param>
    public static void AddButtonListener(Button button, EventTriggerType triggerType, UnityAction callback)
    {
        if (button == null)
            return;

        var trigger = button.GetComponent<EventTrigger>();

        if (trigger == null)
        {
            button.AddComponent<EventTrigger>();
            trigger = button.GetComponent<EventTrigger>();
        }

        (button as CommonButton).buttonCallback += callback;

        EventTrigger.Entry entry = new();
        entry.eventID = triggerType;

        // 케이스 추가되면 계속 처리해줘야하나...??
        if (triggerType == EventTriggerType.PointerDown)
            entry.callback.AddListener(eventData => (button as CommonButton).OnPointerDown((PointerEventData)eventData));
        else if (triggerType == EventTriggerType.PointerUp)
            entry.callback.AddListener(eventData => (button as CommonButton).OnPointerUp((PointerEventData)eventData));
        else if (triggerType == EventTriggerType.PointerEnter)
            entry.callback.AddListener(eventData => (button as CommonButton).OnPointerEnter((PointerEventData)eventData));
        else if (triggerType == EventTriggerType.PointerUp)
            entry.callback.AddListener(eventData => (button as CommonButton).OnPointerExit((PointerEventData)eventData));

        trigger.triggers.Add(entry);
    }

    public static List<GameObject> GetAllObject(string path)
    {
        return Resources.LoadAll<GameObject>(path).ToList();
    }

    public static List<T> GetComponents<T>(Transform parent, string objName) where T : Transform
    {
        return new List<T>();
    }

    public static List<T> GetComponentsInChildren<T>(Transform parent, string objName, bool stringContain = false) where T : Component
    {
        var list = new List<T>();
        var allObjArray = parent.GetComponentsInChildren<T>();

        for (int index = 0; index < allObjArray.Length; index++)
        {
            var item = allObjArray[index];

            if (stringContain)
            {
                if (item.ToString().Contains(objName))
                    list.Add(item);
                else
                    continue;
            }
            else
            {
                if (item.ToString() == objName)
                    list.Add(item);
                else
                    continue;
            }
        }

        return list;
    }

    public static T GetComponent<T>(Transform parent, string objName) where T : Transform
    {
        return parent.Find(objName) as T;
    }

    public static T GetComponentInChildren<T>(Transform parent, string objName) where T : Transform
    {
        return null;
    }

    public static bool IsAllInteger(string text)
    {
        return text.All(char.IsDigit);
    }
}

public static class StringUtil
{
    public static string AddComma(this int thisInt)
    {
        return string.Format("{0:#,###0}", thisInt);
    }
}