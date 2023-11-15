using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

// 저장 / 불러오기 관련
public class JsonManager : SingletonObject<JsonManager>
{
    private const string BASE_PATH = "Assets/Json";

    public const string PLAYER_DATA = "PlayerData";
    public const string SLOT_DATA = "SlotData";

    // PlayerData 하위
    public const string PLAYER_BASE_DATA_FILE_NAME = "PlayerBaseData";
    public const string PLAYER_MESH_DATA_FILE_NAME = "PlayerMeshData";
    public const string PLAYER_INVENTORY_DATA_FILE_NAME = "PlayerInventoryData";

    // SlotData 하위
    public const string SLOT_DATA_FILE_NAME = "SavedSlotData";

    private SlotBaseData _slotBaseData;

    private PlayerBaseData _baseData;
    private PlayerMeshData _meshData;
    private PlayerInventoryData _inventoryData;

    // Properties
    public SlotBaseData SlotBaseData => _slotBaseData;

    public PlayerMeshData MeshData => _meshData;
    public PlayerBaseData BaseData => _baseData;
    public PlayerInventoryData InventoryData => _inventoryData;

    protected override void Awake()
    {
        base.Awake();

        LoadData(SLOT_DATA, SLOT_DATA_FILE_NAME, out _slotBaseData);

        LoadData(PLAYER_DATA, PLAYER_BASE_DATA_FILE_NAME, out _baseData);
        LoadData(PLAYER_DATA, PLAYER_MESH_DATA_FILE_NAME, out _meshData);
        LoadData(PLAYER_DATA, PLAYER_INVENTORY_DATA_FILE_NAME, out _inventoryData);
    }

    public void SaveData<T>(string path, string fileName, T data)
    {
        var jsonData = ObjectToJson(data);
        File.WriteAllText($"{BASE_PATH}/{path}/{fileName}.json", jsonData);

        // 에셋 리프레시
        AssetDatabase.Refresh();
    }

    public void LoadData<T>(string path, string fileName, out T field) where T : new()
    {
        try
        {
            // 저장된 json 파일이 있으면 가져오기
            field = LoadJsonFile<T>(path, fileName);
        }
        catch (Exception e)
        {
            // try 문에서 튕겨져 나왔으면 이유라도 알아야지
            Debug.LogWarning(e.Message);

            // 없으면 새로 생성 후에 다시 로드
            field = new T();

            // 처음 슬롯 파일을 생성할 땐 빈 슬롯(isempty = true)으로 세팅해야함
            if (field.GetType().Equals(typeof(SlotBaseData)))
            {
                var array = (field as SlotBaseData)._isEmpty;

                for (int index = 0; index < array.Length; index++)
                    array[index] = true;
            }

            string jsonData = ObjectToJson(field);
            CreateJsonFile(path, fileName, jsonData);

            field = LoadJsonFile<T>(path, fileName);
        }

        // 에셋 리프레시
        AssetDatabase.Refresh();
    }

    public string ObjectToJson(object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }

    public T JsonToObject<T>(string jsonData)
    {
        return JsonConvert.DeserializeObject<T>(jsonData);
    }

    public void CreateJsonFile(string createPath, string fileName, string jsonData)
    {
        FileStream fileStream = new FileStream($"{BASE_PATH}/{createPath}/{fileName}.json", FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }

    public T LoadJsonFile<T>(string loadPath, string fileName)
    {
        FileStream fileStream = new FileStream($"{BASE_PATH}/{loadPath}/{fileName}.json", FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();
        string jsonData = Encoding.UTF8.GetString(data);

        return JsonConvert.DeserializeObject<T>(jsonData);
    }
}
