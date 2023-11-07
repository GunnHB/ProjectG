using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Sirenix.OdinInspector;
using System;
using Unity.VisualScripting;
using System.Linq;

public class CustomizeHUD : UIHUDBase
{
    public enum MeshCategory
    {
        None,
        Hair,
        Skin,
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
    private PlayerMeshScriptableObject _meshScriptableObject;

    [Title("[Hair]")]
    [TabGroup("RightPanel"), SerializeField] private Button _hairLeftButton;
    [TabGroup("RightPanel"), SerializeField] private Button _hairRightButton;
    [TabGroup("RightPanel"), SerializeField] private TextMeshProUGUI _hairInfoText;

    [Title("[Skin]")]
    [TabGroup("RightPanel"), SerializeField] private Button _skinLeftButton;
    [TabGroup("RightPanel"), SerializeField] private Button _skinRightButton;
    [TabGroup("RightPanel"), SerializeField] private TextMeshProUGUI _skinInfoText;

    private Transform _playerTransform;                             // 회전용
    private Transform _playerMeshTransform;                         // 메시 세팅용

    // 메시 선택 변수
    private int _hairIndex = 0;
    private int _skinIndex = 0;

    // 캐릭터 회전 관련 변수
    private Quaternion _originRotate;                               // 기본 로테이션
    private Coroutine _rotateCoroutine;                             // 회전 코루틴
    private bool _reverse = false;                                  // 위에서 봤을 때 시계 방향이 true // 기본적으로 true

    private Button _curRotateButton = null;                         // 현재 눌리고 있는 회전 버튼

    // 각 부위 별 메시를 구성하기 위한 딕셔너리
    private Dictionary<MeshCategory, Mesh[]> _meshDictionary = new();
    // 메시 소켓 딕셔너리
    private Dictionary<MeshCategory, SkinnedMeshRenderer> _socketDictionary = new();

    protected override void Start()
    {
        base.Start();

        GetPlayerTransform();

        InitMeshDictionary();

        SetLeftButton();
        SetRightButton();
    }

    private void GetPlayerTransform()
    {
        _playerTransform = GameObject.Find("Player").transform;

        if (_playerTransform != null)
            _playerMeshTransform = Util.GetComponent<Transform>(_playerTransform, "NoWeapon01");
    }

    private void InitMeshDictionary()
    {
        if (_playerMeshTransform == null)
            return;

        // Add dictionary
        if (_meshScriptableObject == null)
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

    private void SetLeftButton()
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

    private void SetRightButton()
    {
        // 기존에 있는 skinnedmeshrenderer 의 mesh 가 교체되는 구조

        // Hair
        Util.AddButtonListener(_hairLeftButton, () => OnClickHairButton(false));
        Util.AddButtonListener(_hairRightButton, () => OnClickHairButton(true));
        SetText(_hairInfoText, "Hair", _hairIndex);

        // Skin
        Util.AddButtonListener(_skinLeftButton, () => OnClickSkinButton(false));
        Util.AddButtonListener(_skinRightButton, () => OnClickSkinButton(true));
        SetText(_skinInfoText, "Skin", _skinIndex);
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
}
