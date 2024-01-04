using UnityEngine;
using UnityEngine.UI;

using Sirenix.OdinInspector;

public class HeartObject : MonoBehaviour
{
    private const string IMAGE_GROUP = "[HEART IMAGE]";

    [BoxGroup(IMAGE_GROUP)]
    [SerializeField] private System.Collections.Generic.List<Image> _quaterImageArray;
    
    [SerializeField] private Image _quaterImage;

    public void SetHeart(float value)
    {
        _quaterImage.fillAmount = value;
    }

    public void ResetHeart()
    {
        for(int index = 0; index < _quaterImageArray.Count; index++)
            _quaterImageArray[index].gameObject.SetActive(false);
    }

    public void SetHeart(int count)
    {
        if(_quaterImageArray.Count == 0)
            return;

        ResetHeart();

        for (int index = 0; index < count; index++)
            _quaterImageArray[index].gameObject.SetActive(true);
    }
}
