using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using System.Linq;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public static class Util
{
    /// <summary>
    /// 기본 버튼 리스너
    /// </summary>
    /// <param name="button"></param>
    /// <param name="action"></param>
    public static void AddButtonListener(Button button, UnityAction action)
    {
        if (button == null)
            return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }

    /// <summary>
    /// EventTrigger 기능을 수행하는 버튼 리스너
    /// </summary>
    /// <param name="button"></param>
    /// <param name="action"></param>
    public static void AddButtonListener(Button button, EventTriggerType triggerType, UnityAction<BaseEventData> action)
    {
        if (button == null)
            return;

        var trigger = button.GetComponent<EventTrigger>();

        if (trigger == null)
        {
            button.AddComponent<EventTrigger>();
            trigger = button.GetComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new();
        entry.eventID = triggerType;
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

    /// <summary>
    /// 버튼에 PointerDown, PointerUp 기능 추가해주기
    /// </summary>
    /// <param name="button"></param>
    /// <param name="triggerType"></param>
    /// <param name="action"></param>
    public static void AddUpAndDownEvent(Button button, EventTriggerType triggerType, UnityAction<BaseEventData> action)
    {

    }

    /// <summary>
    /// 버튼에 PointerDrag, PointerDrop 기능 추가하기
    /// </summary>
    /// <param name="button"></param>
    /// <param name="triggerType"></param>
    /// <param name="action"></param>
    public static void AddDragAndDropEvent(Button button, EventTriggerType triggerType, UnityAction<BaseEventData> action)
    {

    }

    /// <summary>
    /// 버튼에 
    /// </summary>
    public static void AddPointerUpButtonListener()
    {

    }

    public static void AddPointerDownListener()
    {

    }

    public static void AddPointerDragListener()
    {

    }

    public static List<GameObject> GetAllObject(string path)
    {
        return Resources.LoadAll<GameObject>(path).ToList();
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