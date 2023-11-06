using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using System.Linq;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using UnityEditor;

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
        return null;
    }

    public static T GetComponentInChildren<T>(Transform parent, string objName) where T : Transform
    {
        return null;
    }
}

public static class SheetUtil
{
    // Item
    private const string TABLE_ITEM_ADDRESS = "https://docs.google.com/spreadsheets/d/1v8CHl9OFoVmz8MncWfcvlIJCKLN6JpX4OjpkXnHJsVg";
    private const string TABLE_ITEM_RANGE = "A3:E";
    private const string TABLE_ITEM_SHEETID = "0";

    // Weapon

    // Armor

    // Food

    public static string GetAddress(string address, string range, string sheetId)
    {
        return $"{address}/export?format=tsv&range={range}&gid={sheetId}";
    }
}

public static class MeshUtil
{
    private const string fbxPath = "Assets/Resources/PackageResource/ModularRPGHeroesPolyArt/Mesh";
    private const string fbxFileName = "CharacterBaseMesh";

    public static void GetMesh()
    {
        ModelImporter model = AssetImporter.GetAtPath(fbxPath) as ModelImporter;
    }
}