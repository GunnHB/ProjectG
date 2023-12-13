using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Sirenix.OdinInspector;
using System;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine.InputSystem;

public class UICustomizePanel : UIPanelBase
{
    // 변경할 메시의 카테고리
    public enum MeshCategory
    {
        None,
        Hair,
        Skin,
    }

    // 해당 허드가 열린 상황
    public enum CustomMode
    {
        None,
        Create,
        Change,
    }

    private const string SOCKET_HAIR = "Socket_Hair";
    private const string SOCKET_SKIN = "Socket_Face";

    [Title("[Buttons]")]
    [TabGroup("LeftPanel"), SerializeField]
    private Button _leftRotateButton;
    [TabGroup("LeftPanel"), SerializeField]
    private Button _resetRotateButton;
    [TabGroup("LeftPanel"), SerializeField]
    private Button _rightRotateButton;

    [Title("[Options]")]
    [TabGroup("LeftPanel"), SerializeField, Range(.1f, 1f)]
    private float _rotateSpeed = .7f;
    [TabGroup("LeftPanel"), SerializeField]
    private GameObject _playerPrefab;
    [TabGroup("LeftPanel"), SerializeField]
    private PlayerMeshScriptableObject _meshScriptableObject;

    [Title("[Hair]")]
    [TabGroup("RightPanel"), SerializeField] private Button _hairLeftButton;
    [TabGroup("RightPanel"), SerializeField] private Button _hairRightButton;
    [TabGroup("RightPanel"), SerializeField] private TextMeshProUGUI _hairInfoText;

    [Title("[Skin]")]
    [TabGroup("RightPanel"), SerializeField] private Button _skinLeftButton;
    [TabGroup("RightPanel"), SerializeField] private Button _skinRightButton;
    [TabGroup("RightPanel"), SerializeField] private TextMeshProUGUI _skinInfoText;

    [Title("[Name input field]")]
    [TabGroup("RightPanel"), SerializeField] private GameObject _nameInputFieldParent;
    [TabGroup("RightPanel"), SerializeField] private TMP_InputField _nameInputField;

    [Title("[Button]")]
    [TabGroup("RightPanel"), SerializeField] private Button _backButton;
    [TabGroup("RightPanel"), SerializeField] private Button _confirmButton;

    private Transform _playerTransform;                             // 회전용
    private Transform _playerMeshTransform;                         // 메시 세팅용

    // 메시 선택 변수
    private int _hairIndex = 0;
    private int _skinIndex = 0;

    // 캐릭터 회전 관련 변수
    private Quaternion _originRotate;                               // 기본 로테이션
    private Coroutine _rotateCoroutine;                             // 회전 코루틴
    private bool _reverse = false;                                  // 시계 방향이 true

    private Button _curRotateButton = null;                         // 현재 눌리고 있는 회전 버튼

    // 각 부위 별 메시를 구성하기 위한 딕셔너리
    private Dictionary<MeshCategory, Mesh[]> _meshDictionary = new();
    // 메시 소켓 딕셔너리
    private Dictionary<MeshCategory, SkinnedMeshRenderer> _socketDictionary = new();

    // 현재 커스텀 허드의 상태
    private CustomMode _currentCustomMode = CustomMode.None;
    public CustomMode CurrentCustomMode => _currentCustomMode;

    private Mesh _currHairMesh = null;
    private Mesh _currSkinMesh = null;

    private Vector3 _initPosition = new Vector3(0, -1000, 0);
    private Quaternion _initRotation = Quaternion.Euler(new Vector3(0, 180, 0));

    #region 캐싱
    private int _slotIndex = GameManager.Instance.SelectedSlotIndex;

    private string _slotDataPath = JsonManager.SLOT_DATA;
    private string _slotDataFileName = JsonManager.SLOT_DATA_FILE_NAME;

    private string _playerBaseDataPath = JsonManager.PLAYER_DATA;
    private string _playerBaseDataFileName = JsonManager.PLAYER_BASE_DATA_FILE_NAME;
    private string _playerMeshDataFileName = JsonManager.PLAYER_MESH_DATA_FILE_NAME;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        SetButtons();
    }

    private void SetButtons()
    {
        // left panel
        {
            Util.AddButtonListenerV2(_resetRotateButton, OnClickResetRotateButton);

            Util.AddPressButtonListener(_leftRotateButton, () =>
            {
                _curRotateButton = _leftRotateButton;
                _reverse = true;

                OnClickRotateButton();
            });

            Util.AddPressButtonListener(_rightRotateButton, () =>
            {
                _curRotateButton = _rightRotateButton;
                _reverse = false;

                OnClickRotateButton();
            });
        }

        // right panel
        {
            // skinnedmeshrenderer 의 mesh 가 교체되는 구조

            // Hair
            Util.AddButtonListener(_hairLeftButton, () => OnClickHairButton(false));
            Util.AddButtonListener(_hairRightButton, () => OnClickHairButton(true));

            // Skin
            Util.AddButtonListener(_skinLeftButton, () => OnClickSkinButton(false));
            Util.AddButtonListener(_skinRightButton, () => OnClickSkinButton(true));

            // Button
            Util.AddButtonListener(_backButton, OnClickBackButton);
            Util.AddButtonListener(_confirmButton, OnClickConfirmButton);
        }
    }


    public void Init()
    {
        GetPlayerTransform();

        InitMeshDictionary();

        SetLeftPanel();
        SetRightPanel();
    }

    private void GetPlayerTransform()
    {
        // 이미 생성됐으면 굳이 생성하지 않음
        if (_playerTransform == null)
            _playerTransform = Instantiate(_playerPrefab.transform, this.transform);

        if (_playerTransform == null)
            return;

        if (_playerTransform.TryGetComponent(out PlayerController controller))
            controller.enabled = false;

        if (_playerTransform.TryGetComponent(out PlayerInput input))
            input.enabled = false;

        _playerMeshTransform = Util.GetComponent<Transform>(_playerTransform, "NoWeapon01");
    }

    private void ResetPlayerTransform()
    {
        _playerTransform.localPosition = _initPosition;
        _playerTransform.rotation = _initRotation;

        _originRotate = _playerTransform.rotation;

        _hairIndex = 0;
        _skinIndex = 0;

        _nameInputField.text = string.Empty;
    }

    private void InitMeshDictionary()
    {
        if (_playerMeshTransform == null || _meshScriptableObject == null)
            return;

        _meshDictionary.Clear();
        _socketDictionary.Clear();

        GetSkinnedMeshRenderer(MeshCategory.Hair, SOCKET_HAIR);
        GetSkinnedMeshRenderer(MeshCategory.Skin, SOCKET_SKIN);

        AddMeshDictionary(MeshCategory.Hair, _meshScriptableObject._hairMeshArray);
        AddMeshDictionary(MeshCategory.Skin, _meshScriptableObject._skinMeshArray);

        // 초기화
        _currHairMesh = _meshScriptableObject._hairMeshArray[0];
        _currSkinMesh = _meshScriptableObject._skinMeshArray[0];
    }

    private void GetSkinnedMeshRenderer(MeshCategory meshCategory, string _objName)
    {
        for (int index = 0; index < _playerMeshTransform.childCount; index++)
        {
            var item = _playerMeshTransform.GetChild(index);

            if (item.TryGetComponent(out SkinnedMeshRenderer meshRenderer))
            {
                if (item.name != _objName)
                    continue;

                if (!_socketDictionary.ContainsKey(meshCategory))
                    _socketDictionary.Add(meshCategory, meshRenderer);
                else
                    _socketDictionary[meshCategory] = meshRenderer;
            }
        }
    }

    private void AddMeshDictionary(MeshCategory cate, Mesh[] meshArray)
    {
        if (_meshDictionary.ContainsKey(cate))
            _meshDictionary[cate] = meshArray;
        else
            _meshDictionary.Add(cate, meshArray);
    }

    private void SetLeftPanel()
    {
        if (_playerTransform == null)
            return;

        ResetPlayerTransform();

        SetMesh(MeshCategory.Hair, _hairIndex, out _currHairMesh);
        SetMesh(MeshCategory.Skin, _skinIndex, out _currSkinMesh);
    }

    private void SetRightPanel()
    {
        // skinnedmeshrenderer 의 mesh 가 교체되는 구조

        // Hair
        SetText(_hairInfoText, "Hair", _hairIndex);

        // Skin
        SetText(_skinInfoText, "Skin", _skinIndex);

        // nickname 캐릭터 생성이 아닐 땐 보일 필요 없음
        ActiveInputField();
    }

    private void OnClickRotateButton()
    {
        if (_rotateCoroutine != null)
        {
            StopCoroutine(_rotateCoroutine);
            _rotateCoroutine = null;
        }

        _rotateCoroutine = StartCoroutine(nameof(Cor_Rotate));
    }

    private void OnClickResetRotateButton()
    {
        // 원위치 돼있는데 굳이...
        if (_playerTransform.rotation.eulerAngles.y == _originRotate.eulerAngles.y)
            return;

        _playerTransform.rotation = _originRotate;
    }

    private IEnumerator Cor_Rotate()
    {
        int multiValue = _reverse ? 1 : -1;
        var button = _curRotateButton as CommonButton;

        if (button == null)
            yield break;

        while (button.IsPress)
        {
            // 마지막 100은 보정 값
            _playerTransform.Rotate(Vector3.up * _rotateSpeed * multiValue * Time.deltaTime * 100f);
            yield return null;
        }
    }

    private void OnClickHairButton(bool next)
    {
        SetIndex(next, ref _hairIndex, MeshCategory.Hair);

        // _currHairMesh = _meshDictionary[MeshCategory.Hair][_hairIndex];
        // _socketDictionary[MeshCategory.Hair].sharedMesh = _currHairMesh;

        SetMesh(MeshCategory.Hair, _hairIndex, out _currHairMesh);

        SetText(_hairInfoText, "Hair", _hairIndex);
    }

    private void OnClickSkinButton(bool next)
    {
        SetIndex(next, ref _skinIndex, MeshCategory.Skin);

        // _currSkinMesh = _meshDictionary[MeshCategory.Skin][_skinIndex];
        // _socketDictionary[MeshCategory.Skin].sharedMesh = _currSkinMesh;

        SetMesh(MeshCategory.Skin, _skinIndex, out _currSkinMesh);

        SetText(_skinInfoText, "Skin", _skinIndex);
    }

    private void SetMesh(MeshCategory cate, int index, out Mesh currMesh)
    {
        currMesh = _meshDictionary[cate][index];
        _socketDictionary[cate].sharedMesh = currMesh;
    }

    private void SetIndex(bool next, ref int index, MeshCategory cate)
    {
        index = next ? ++index : --index;

        if (_meshDictionary.TryGetValue(cate, out var array))
        {
            int lastIndex = array.Length - 1;

            if (index < 0)
                index = lastIndex;
            else if (index > lastIndex)
                index = 0;
        }
    }

    private void SetText(TextMeshProUGUI text, string defaultText, int index)
    {
        text.text = $"{defaultText}_{index + 1}";
    }

    private void ActiveInputField()
    {
        _nameInputFieldParent.SetActive(GameManager.Instance.CurrentMode == GameManager.GameMode.Title);
    }

    private void OnClickBackButton()
    {
        if (GameManager.Instance.CurrentMode == GameManager.GameMode.Title)
        {
            string title = "알림";
            string message = "캐릭터 선택 화면으로 이동하시겠습니까?";
            string confirm = "확인";
            string cancel = "취소";

            // 씬 이동하거나 같은 씬에서 해당 허드만 종료 시키는 기능 추가하면 될 듯...?
            UIManager.Instance.OpenSimpleDialogue(title, message,
                                                  confirm, BackToSelectScharacter,
                                                  cancel, null);
        }
    }

    private void OnClickConfirmButton()
    {
        if (GameManager.Instance.CurrentMode == GameManager.GameMode.Title)
        {
            // 나중에 토스트 추가하자
            if (_nameInputField.text == string.Empty)
            {
                Debug.Log("닉네임을 입력해주세요.");
                return;
            }

            if (Util.IsAllInteger(_nameInputField.text))
                Debug.Log("문자나 문자+숫자 조합으로 이름을 정해주세요.");
            else
            {
                string title = "알림";
                string message = $"[<color=#FFCC70>{_nameInputField.text}</color>]\n\n해당 이름으로 시작하시겠습니까?";

                UIManager.Instance.OpenSimpleDialogue(title, message, confirmAction: CreateNewCharacter, cancelAction: null);
            }
        }
    }

    // 캐릭터 생성 버튼 콜백
    private void CreateNewCharacter()
    {
        SavePlayerBaseData();
        SavePlayerMeshData();

        // 빈 슬롯 여부 저장
        JsonManager.Instance.SlotBaseData._isEmpty[_slotIndex] = false;
        JsonManager.Instance.SaveData(_slotDataPath, _slotDataFileName, JsonManager.Instance.SlotBaseData);

        LoadSceneManager.Instance.FadeInOut(null, LoadSceneManager.SceneType.InGame);
    }

    private void SavePlayerMeshData()
    {
        JsonManager.Instance.MeshData._hairHesh[_slotIndex] = _currHairMesh;
        JsonManager.Instance.MeshData._skinMesh[_slotIndex] = _currSkinMesh;

        JsonManager.Instance.SaveDataByJsonUtility(_playerBaseDataPath, _playerMeshDataFileName, JsonManager.Instance.MeshData);
    }

    private void SavePlayerBaseData()
    {
        JsonManager.Instance.BaseData._playerName[_slotIndex] = _nameInputField.text;
        JsonManager.Instance.BaseData._playerHP[_slotIndex] = GameValue.INIT_HP;
        JsonManager.Instance.BaseData._playerStamina[_slotIndex] = GameValue.INIT_STAMINA;

        JsonManager.Instance.SaveData(_playerBaseDataPath, _playerBaseDataFileName, JsonManager.Instance.BaseData);
    }

    private void BackToSelectScharacter()
    {
        LoadSceneManager.Instance.FadeInOut(BackButtonAction);
    }

    private void BackButtonAction()
    {
        this.gameObject.SetActive(false);

        var openedUI = UIManager.Instance.FindOpendUI<UISelectCharacterPanel>(UIManager.Instance.PanelCanvas);

        if (openedUI != null)
            openedUI.gameObject.SetActive(true);
        else
            openedUI = UIManager.Instance.OpenUI<UISelectCharacterPanel>("SelectCharacterPanel/UISelectCharacterPanel");

        openedUI.Init();
    }
}
