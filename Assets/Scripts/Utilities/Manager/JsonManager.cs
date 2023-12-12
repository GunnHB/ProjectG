using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
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
        // LoadData(PLAYER_DATA, PLAYER_MESH_DATA_FILE_NAME, out _meshData);
        LoadData(PLAYER_DATA, PLAYER_INVENTORY_DATA_FILE_NAME, out _inventoryData);

        LoadDataByJsonUtility(PLAYER_DATA, PLAYER_MESH_DATA_FILE_NAME, out _meshData);
    }

    public void CreateJsonFile(string createPath, string fileName, string jsonData)
    {
        FileStream fileStream = new FileStream($"{BASE_PATH}/{createPath}/{fileName}.json", FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }

    public T LoadJsonFile<T>(string loadPath, string fileName, bool isNewtonSoft = true)
    {
        FileStream fileStream = new FileStream($"{BASE_PATH}/{loadPath}/{fileName}.json", FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();
        string jsonData = Encoding.UTF8.GetString(data);

        if (isNewtonSoft)
            return JsonToObject<T>(jsonData);
        else
            return JsonToObjectByJsonUtility<T>(jsonData);
    }

    // ================================================================
    // 하위의 json 저장은 newtonsoft.json을 사용합니다.
    // ================================================================
    public void SaveData<T>(string path, string fileName, T data)
    {
        var jsonData = ObjectToJson(data);
        File.WriteAllText($"{BASE_PATH}/{path}/{fileName}.json", jsonData);
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
    }

    public string ObjectToJson(object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }

    public T JsonToObject<T>(string jsonData)
    {
        return JsonConvert.DeserializeObject<T>(jsonData);
    }

    // ================================================================
    // 하위의 json 저장은 jsonutility를 사용합니다.
    // ================================================================
    public void SaveDataByJsonUtility<T>(string path, string fileName, T data)
    {
        var jsonData = ObjectToJsonByJsonUtility(data);
        File.WriteAllText($"{BASE_PATH}/{path}/{fileName}.json", jsonData);
    }

    // 기존의 LoadData 와 형태는 거의 동일합니다.
    public void LoadDataByJsonUtility<T>(string path, string fileName, out T field) where T : new()
    {
        try
        {
            field = LoadJsonFile<T>(path, fileName, false);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);

            field = new T();

            string jsonData = ObjectToJsonByJsonUtility(field);
            CreateJsonFile(path, fileName, jsonData);

            field = LoadJsonFile<T>(path, fileName, false);
        }
    }

    public string ObjectToJsonByJsonUtility(object obj)
    {
        // serialize 함다
        return JsonUtility.ToJson(obj);
    }

    public T JsonToObjectByJsonUtility<T>(string jsonData)
    {
        // deserialize 함다
        return JsonUtility.FromJson<T>(jsonData);
    }
}
