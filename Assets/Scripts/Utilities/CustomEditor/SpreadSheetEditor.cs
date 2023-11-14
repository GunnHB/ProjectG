using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

using UnityEditor;
using UnityEngine.Networking;
using Sirenix.Utilities;

using Unity.EditorCoroutines.Editor;
using Unity.VisualScripting;
using System;
using System.Reflection;

public class SpreadSheetInfo
{
    public string _itemAddress;
    public string _itemRange;
    public string _itemSheetId;
}

public class SpreadSheetEditor : OdinEditorWindow
{
    [MenuItem("Tools/CustomEditors/SpreadSheetEditor")]
    private static void Open()
    {
        GetWindow<SpreadSheetEditor>();
    }

    [SerializeField]
    private Dictionary<string, SpreadSheetInfo> _spreasSheetDic = new();

    private EditorCoroutine _webRequestCoroutine;

    [Button(ButtonSizes.Gigantic)]
    private void GetTSVAddress(string tableName)
    {
        if (string.IsNullOrEmpty(tableName) || !_spreasSheetDic.ContainsKey(tableName))
        {
            Debug.LogWarning("테이블 이름을 확인해주세요!!");
            return;
        }

        if (_webRequestCoroutine != null)
        {
            EditorCoroutineUtility.StopCoroutine(_webRequestCoroutine);
            _webRequestCoroutine = null;
        }

        _webRequestCoroutine = EditorCoroutineUtility.StartCoroutine(Cor_WebRequest(tableName), this);
    }

    private IEnumerator Cor_WebRequest(string tableName)
    {
        UnityWebRequest www = UnityWebRequest.Get(GetAdress(tableName));
        yield return www.SendWebRequest();

        // Debug.Log(www.downloadHandler.text);
        // Debug.Log(GetData<ModelItem>(www.downloadHandler.text.Split('\n')[0].Split('\t'))._itemType);
    }

    private string GetAdress(string tableName)
    {
        string address = _spreasSheetDic[tableName]._itemAddress;
        string range = _spreasSheetDic[tableName]._itemRange;
        string sheetId = _spreasSheetDic[tableName]._itemSheetId;

        return $"{address}/export?format=tsv&range={range}&gid={sheetId}";
    }

    private List<T> GetDatas<T>(string data)
    {
        List<T> returnList = new();
        string[] splitDatas = data.Split('\n');

        foreach (var item in splitDatas)
        {
            string[] datas = item.Split('\t');
            returnList.Add(GetData<T>(datas));
        }

        return returnList;
    }

    private T GetData<T>(string[] datas)
    {
        object data = Activator.CreateInstance(typeof(T));

        FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        for (int index = 0; index < fields.Length; index++)
        {
            try
            {
                Type type = fields[index].FieldType;

                if (string.IsNullOrEmpty(datas[index]))
                    continue;

                // 타입에 맞게 분류
                if (type == typeof(int))
                    fields[index].SetValue(data, int.Parse(datas[index]));
                else if (type == typeof(long))
                    fields[index].SetValue(data, long.Parse(datas[index]));
                else if (type == typeof(float))
                    fields[index].SetValue(data, float.Parse(datas[index]));
                else if (type == typeof(bool))
                    fields[index].SetValue(data, bool.Parse(datas[index]));
                else if (type == typeof(string))
                    fields[index].SetValue(data, datas[index]);
                // 여기는 enum
                else
                    fields[index].SetValue(data, Enum.Parse(type, datas[index]));
            }
            catch (Exception e)
            {
                Debug.LogError($"SpreadSheet Error : {e.Message}");
            }
        }

        return (T)data;
    }
}
