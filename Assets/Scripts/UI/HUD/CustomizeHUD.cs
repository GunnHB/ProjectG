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

public class CustomizeHUD : UIHUDBase
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

    protected override void Awake()
    {
        base.Awake();
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
        var initPosition = new Vector3(0, -1000, 0);
        var initRotation = Quaternion.Euler(new Vector3(0, 180, 0));

        _playerTransform = Instantiate(_playerPrefab.transform, this.transform);

        if (_playerTransform == null)
            return;

        if (_playerTransform.TryGetComponent(out PlayerController controller))
            controller.enabled = false;

        if (_playerTransform.TryGetComponent(out PlayerInput input))
            input.enabled = false;

        _playerMeshTransform = Util.GetComponent<Transform>(_playerTransform, "NoWeapon01");

        _playerTransform.localPosition = initPosition;
        _playerTransform.rotation = initRotation;
    }

    private void InitMeshDictionary()
    {
        if (_playerMeshTransform == null || _meshScriptableObject == null)
            return;

        GetSkinnedMeshRenderer(MeshCategory.Hair, SOCKET_HAIR);
        GetSkinnedMeshRenderer(MeshCategory.Skin, SOCKET_SKIN);

        AddMeshDictionary(MeshCategory.Hair, _meshScriptableObject._hairMeshArray);
        AddMeshDictionary(MeshCategory.Skin, _meshScriptableObject._skinMeshArray);
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
        if (_playerTransform != null)
            _originRotate = _playerTransform.rotation;

        Util.AddButtonListener(_leftRotateButton, () =>
        {
            _curRotateButton = _leftRotateButton;
            _reverse = true;

            OnClickRotateButton();
        });

        Util.AddButtonListener(_rightRotateButton, () =>
        {
            _curRotateButton = _rightRotateButton;
            _reverse = false;

            OnClickRotateButton();
        });

        Util.AddButtonListener(_resetRotateButton, OnClickResetRotateButton);
    }

    private void SetRightPanel()
    {
        // skinnedmeshrenderer 의 mesh 가 교체되는 구조

        // Hair
        Util.AddButtonListener(_hairLeftButton, () => OnClickHairButton(false));
        Util.AddButtonListener(_hairRightButton, () => OnClickHairButton(true));
        SetText(_hairInfoText, "Hair", _hairIndex);

        // Skin
        Util.AddButtonListener(_skinLeftButton, () => OnClickSkinButton(false));
        Util.AddButtonListener(_skinRightButton, () => OnClickSkinButton(true));
        SetText(_skinInfoText, "Skin", _skinIndex);

        // nickname 캐릭터 생성이 아닐 땐 보일 필요 없음
        ActiveInputField();

        // Button
        Util.AddButtonListener(_backButton, OnClickBackButton);
        Util.AddButtonListener(_confirmButton, OnClickConfirmButton);
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
        _socketDictionary[MeshCategory.Hair].sharedMesh = _meshDictionary[MeshCategory.Hair][_hairIndex];

        SetText(_hairInfoText, "Hair", _hairIndex);
    }

    private void OnClickSkinButton(bool next)
    {
        SetIndex(next, ref _skinIndex, MeshCategory.Skin);
        _socketDictionary[MeshCategory.Skin].sharedMesh = _meshDictionary[MeshCategory.Skin][_skinIndex];

        SetText(_skinInfoText, "Skin", _skinIndex);
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
            string message = "타이틀 화면으로 이동하시겠습니까?";
            string confirm = "확인";
            string cancel = "취소";

            // 씬 이동하거나 같은 씬에서 해당 허드만 종료 시키는 기능 추가하면 될 듯...?
            UIManager.Instance.OpenCommonDialogue(title, message,
                                                  confirm, null,
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

                UIManager.Instance.OpenCommonDialogue(title, message, confirmAction: CreateNewCharacter, cancelAction: null);
            }
        }
    }

    // 캐릭터 생성 버튼 콜백
    private void CreateNewCharacter()
    {
        // JsonManager.Instance.CreateJsonFile("PlayerCustomData", "TestPlayer", "");
    }
}
