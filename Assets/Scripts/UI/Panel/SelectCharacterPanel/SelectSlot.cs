using TMPro;

using UnityEngine;
using UnityEngine.UI;

using Sirenix.OdinInspector;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class SelectSlot : MonoBehaviour
{
    public enum SeparateType
    {
        Left,
        Center,
        Right,
    }

    private const string BASE_FILE_NAME = "SelectPlayerRenderTexture_";

    [Title("[Text]")]
    [SerializeField] private TextMeshProUGUI _playerName;

    [Title("[GameObject]")]
    [SerializeField] private GameObject _containGroup;
    [SerializeField] private GameObject _buttonGroup;

    [Title("[Button]")]
    [SerializeField] private Button _createButton;
    [SerializeField] private Button _deleteButton;
    [SerializeField] private Button _playButton;

    [Title("[Player]")]
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Camera _camera;
    [SerializeField] private RawImage _playerRawImage;

    private SeparateType _currentType;
    private UnityAction _closePanelAction;

    private bool _emptyState = false;

    private Vector3 _playerPosition = new Vector3(0, -1000, 0);
    private Quaternion _playerRotation = Quaternion.Euler(new Vector3(0, 180, 0));

    private GameObject _playerPrefab = null;

    #region 캐싱
    private Mesh _hairMesh { get => JsonManager.Instance.MeshData._hairHesh[(int)_currentType]; }
    private Mesh _skinMesh { get => JsonManager.Instance.MeshData._skinMesh[(int)_currentType]; }
    #endregion

    public void InitSlot(SeparateType type, UnityAction callback)
    {
        ResetData();

        _currentType = type;
        _closePanelAction = callback;

        RefreshSlot();
        SetButtonListener();
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

    private void SetPlayerPrefab()
    {
        // 빈 슬롯은 할 필요가 없음다.
        if (_emptyState)
        {
            if (_playerPrefab != null)
                _playerPrefab.SetActive(false);
            return;
        }

        if (_playerPrefab == null)
            _playerPrefab = Instantiate(_prefab, _playerPosition, _playerRotation, this.transform);

        if (_playerPrefab != null)
        {
            // 불필요한 컴포넌트는 끄기
            if (_playerPrefab.TryGetComponent(out PlayerController controller))
                controller.enabled = false;

            if (_playerPrefab.TryGetComponent(out PlayerInput input))
                input.enabled = false;

            if (_playerPrefab.TryGetComponent(out PlayerSkinnedMesh skinnedMeshData))
            {
                skinnedMeshData.SetPlayerSkinnedMesh(PlayerSkinnedMesh.SKINNED_MESH_HAIR, _hairMesh);
                skinnedMeshData.SetPlayerSkinnedMesh(PlayerSkinnedMesh.SKINNED_MESH_SKIN, _skinMesh);
            }

            if (_playerRawImage != null)
            {
                string path = ResourceManager.SELECT_CHARACTER_PATH;
                string fileName = $"{BASE_FILE_NAME}{_currentType}";

                var texture = ResourceManager.Instance.GetRenderTexture(path, fileName);

                if (texture != null)
                {
                    _playerRawImage.texture = texture;
                    _camera.targetTexture = texture;
                }
            }

            _playerPrefab.SetActive(true);
        }
    }

    private void SetCamera()
    {
        _camera.gameObject.SetActive(!_emptyState);
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
        string titleString = "알림";
        string msgString = $"[<color=#FFCC70>{_playerName.text}</color>]\n\n" +
                            "해당 캐릭터가 삭제됩니다.\n" +
                            "진행 하시겠습니까?";
        string confirmString = "삭제";
        string cancelString = "취소";

        UIManager.Instance.OpenSimpleDialogue(titleString, msgString,
                                              confirmString, DeleteData,
                                              cancelString, null);
    }

    private void DeleteData()
    {
        // 빈 슬롯으로 돌림
        JsonManager.Instance.SlotBaseData._isEmpty[(int)_currentType] = true;
        JsonManager.Instance.SaveData(JsonManager.SLOT_DATA, JsonManager.SLOT_DATA_FILE_NAME, JsonManager.Instance.SlotBaseData);

        // 슬롯 리프레시
        RefreshSlot();
    }

    private void OnClickPlayButton()
    {
        string titleString = "알림";
        string msgString = $"[<color=#FFCC70>{_playerName.text}</color>]\n\n" +
                            "해당 캐릭터로 진행하시겠습니까?\n";
        string confirmString = "시작";
        string cancelString = "취소";

        UIManager.Instance.OpenSimpleDialogue(titleString, msgString,
                                              confirmString, StartPlay,
                                              cancelString, () => GameManager.Instance.SetSelectedSlotIndex((int)_currentType));
    }

    private void StartPlay()
    {
        GameManager.Instance.SetSelectedSlotIndex((int)_currentType);
        // 게임 씬으로 전환
        LoadSceneManager.Instance.FadeInOut(GameManager.Instance.LoadPlayerCharacter, LoadSceneManager.SceneType.InGame);
    }

    private void ResetData()
    {
        _closePanelAction = null;
    }

    private void RefreshSlot()
    {
        _emptyState = JsonManager.Instance.SlotBaseData._isEmpty[(int)_currentType];

        SetSlotState();
        SetPlayerNameText();
        SetPlayerPrefab();
        SetCamera();
    }
}
