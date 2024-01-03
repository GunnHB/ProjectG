using UnityEngine;
using UnityEngine.UI;

using Sirenix.OdinInspector;

public class HeartObject : MonoBehaviour
{
    private const string IMAGE_GROUP = "[HEART IMAGE GROUP]";

    [BoxGroup(IMAGE_GROUP)]
    [SerializeField] private Image _oneQuatersImage;
    [BoxGroup(IMAGE_GROUP)]
    [SerializeField] private Image _twoQuatersImage;
    [BoxGroup(IMAGE_GROUP)]
    [SerializeField] private Image _threeQuatersImage;
    [BoxGroup(IMAGE_GROUP)]
    [SerializeField] private Image _fourQuatersImage;

    public void SetHeart(float value)
    {
        _oneQuatersImage.fillAmount = value;
    }
}
