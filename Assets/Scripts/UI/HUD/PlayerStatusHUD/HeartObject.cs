using UnityEngine;
using UnityEngine.UI;

using Sirenix.OdinInspector;

public class HeartObject : MonoBehaviour
{
    [SerializeField] private Image _heartImage;

    public void SetHeart(float value)
    {
        _heartImage.fillAmount = value;
    }
}
