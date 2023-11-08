using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using Sirenix.OdinInspector;

public class SelectSlot : MonoBehaviour
{
    public enum SeparateType
    {
        None,
        Left,
        Center,
        Right,
    }

    private SeparateType _currentType;

    [Title("[Text]")]
    [SerializeField] private TextMeshProUGUI _playerName;

    [Title("[GameObject]")]
    [SerializeField] private GameObject _containGroup;
    [SerializeField] private GameObject _buttonGroup;

    [Title("[Button]")]
    [SerializeField] private Button _createButton;
    [SerializeField] private Button _deleteButton;
    [SerializeField] private Button _playButton;

    public void InitSlot(SeparateType type)
    {
        _currentType = type;
    }
}
