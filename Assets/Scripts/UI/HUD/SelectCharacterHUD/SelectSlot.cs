using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using Sirenix.OdinInspector;
using UnityEngine.Events;

public class SelectSlot : MonoBehaviour
{
    public enum SeparateType
    {
        Left,
        Center,
        Right,
    }

    [Title("[Text]")]
    [SerializeField] private TextMeshProUGUI _playerName;

    [Title("[GameObject]")]
    [SerializeField] private GameObject _containGroup;
    [SerializeField] private GameObject _buttonGroup;

    [Title("[Button]")]
    [SerializeField] private Button _createButton;
    [SerializeField] private Button _deleteButton;
    [SerializeField] private Button _playButton;

    private SeparateType _currentType;
    private UnityAction _closePanelAction;

    private bool _emptyState = false;

    public void InitSlot(SeparateType type, bool isEmpty, UnityAction callback)
    {
        ResetData();

        _currentType = type;
        _emptyState = isEmpty;
        _closePanelAction = callback;

        SetSlotState();
        SetButtonListener();

        SetPlayerNameText();
    }

    private void SetSlotState()
    {
        _containGroup.SetActive(!_emptyState);
        _buttonGroup.SetActive(!_emptyState);

        _createButton.gameObject.SetActive(_emptyState);
    }

    private void SetPlayerNameText()
    {
        // 빈 슬롯이면 실행할 필요 없음
        if (_emptyState)
            return;

        _playerName.text = JsonManager.Instance.BaseData._playerName[(int)_currentType];
    }

    private void SetButtonListener()
    {
        Util.AddButtonListener(_createButton, OnClickCreateButton);
        Util.AddButtonListener(_deleteButton, OnClickDeleteButton);
        Util.AddButtonListener(_playButton, OnClickPlayButton);
    }

    private void OnClickCreateButton()
    {
        GameManager.Instance.SetSelectedSlotIndex((int)_currentType);

        _closePanelAction?.Invoke();
    }

    private void OnClickDeleteButton()
    {

    }

    private void OnClickPlayButton()
    {

    }

    private void ResetData()
    {
        _closePanelAction = null;
    }
}
