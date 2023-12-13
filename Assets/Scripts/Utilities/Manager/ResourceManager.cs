using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.Text;
using Mono.Cecil;

public class ResourceManager : SingletonObject<ResourceManager>
{
    private const string WEAPON_PREFAB_PATH = "Prefabs/Item/Weapon/";
    private const string ARMOR_PREFAB_PATH = "Prefabs/Item/Armor/";
    private const string FOOD_PREFAB_PATH = "Prefabs/Item/Food/";
    private const string DEFAULT_PREFAB_PATH = "Prefabs/Item/Default/";

    public const string PLAYER_PREFAB_PATH = "Prefabs/Characters/Hero/";

    public const string SELECT_CHARACTER_PATH = "UI/RenderTexture/SelectCharacterHUD/";

    protected override void Awake()
    {
        base.Awake();
    }

    public Sprite GetSpriteByItem(ItemType type, string fileName)
    {
        StringBuilder builder = new();
        builder.Append("Inventory/Item/");

        switch (type)
        {
            case ItemType.Armor:
                builder.Append("Armor/");
                break;
            case ItemType.Weapon:
                builder.Append("Weapon/");
                break;
            case ItemType.Food:
                builder.Append("Food/");
                break;
            case ItemType.Default:
                builder.Append("Default/");
                break;
        }

        builder.Append(fileName);

        return GetSprite(builder.ToString());
    }

    public Sprite GetSpriteByIcon(string fileName)
    {
        StringBuilder builder = new();
        builder.Append($"Inventory/Icon/{fileName}");

        return GetSprite(builder.ToString());
    }

    // 나중에 아틀라스로 관리하는 것도 추가합시다.
    public Sprite GetSprite(string path)
    {
        Sprite sprite = Resources.Load<Sprite>($"UI/Sprites/{path}");

        if (sprite != null)
            return sprite;
        else
        {
            Debug.LogWarning("There is no Sprite! Please check the path!");
            return null;
        }
    }

    public Texture GetTexture(string path)
    {
        Texture texture = Resources.Load<Texture>(path);

        if (texture != null)
            return texture;
        else
        {
            Debug.LogWarning("There is no texture! Please check the path!");
            return null;
        }
    }

    public T GetPrefab<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    public T GetWeaponPrefab<T>(string prefabName) where T : Object
    {
        return GetPrefab<T>($"{WEAPON_PREFAB_PATH}{prefabName}");
    }

    public T GetPlayerPrefab<T>(string prefabName) where T : Object
    {
        return GetPrefab<T>($"{PLAYER_PREFAB_PATH}{prefabName}");
    }

    public RenderTexture GetRenderTexture(string path, string fileName)
    {
        RenderTexture render = Resources.Load<RenderTexture>($"{path}{fileName}");

        if (render == null)
        {
            Debug.LogWarning("There is no texture! Please check the path!");
            return null;
        }
        else
            return render;
    }
}