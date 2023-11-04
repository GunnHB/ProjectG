using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class ResourceManager : SingletonObject<ResourceManager>
{
    protected override void Awake()
    {
        base.Awake();
    }

    public Sprite GetSprite(string path)
    {
        Sprite sprite = Resources.Load<Sprite>(path);

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
}