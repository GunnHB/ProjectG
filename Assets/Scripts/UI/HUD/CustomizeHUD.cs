using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Sirenix.OdinInspector;
using System;

public class CustomizeHUD : UIHUDBase
{
    public enum MeshCaetgory
    {
        None,
        Head,
        Skin,
    }

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

    [Title("[Hair]")]
    [TabGroup("RightPanel"), SerializeField] private Button _hairLeftButton;
    [TabGroup("RightPanel"), SerializeField] private Button _hairRightButton;
    [TabGroup("RightPanel"), SerializeField] private TextMeshProUGUI _hairInfoText;

    [Title("[Skin]")]
    [TabGroup("RightPanel"), SerializeField] private Button _skinLeftButton;
    [TabGroup("RightPanel"), SerializeField] private Button _skinRightButton;
    [TabGroup("RightPanel"), SerializeField] private TextMeshProUGUI _skinInfoText;

    private Transform _playerTransform;

    // 메시 선택 변수
    private int _hairIndex = 0;
    private int _skinIndex = 0;

    // 캐릭터 회전 관련 변수
    private Quaternion _originRotate;           // 기본 로테이션
    private Coroutine _rotateCoroutine;         // 회전 코루틴
    private bool _reverse = false;              // 위에서 봤을 때 시계 방향이 true // 기본적으로 true

    private Button _curRotateButton = null;     // 현재 눌리고 있는 회전 버튼

    // 각 부위 별 메시를 구성하기 위한 딕셔너리
    private Dictionary<MeshCaetgory, List<Mesh>> _meshDictionary = new();

    protected override void Start()
    {
        base.Start();

        InitMeshDictionary();

        SetLeftButton();
        SetRightButton();
    }

    private void InitMeshDictionary()
    {
        _playerTransform = GameObject.Find("Player").transform;

        GetSkinnedMeshRenderer("Hair");
        GetSkinnedMeshRenderer("Face");
    }

    private void GetSkinnedMeshRenderer(string partName)
    {
        var hairMeshRenderer = Util.GetComponentsInChildren<SkinnedMeshRenderer>(_playerTransform, partName, true);
    }

    // private void 

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
        // Hair
        Util.AddButtonListener(_hairLeftButton, () => OnClickHairButton(_hairIndex, false));
        Util.AddButtonListener(_hairRightButton, () => OnClickHairButton(_hairIndex, true));

        // Skin
        Util.AddButtonListener(_skinLeftButton, () => OnClickSkinButton(_skinIndex, false));
        Util.AddButtonListener(_skinRightButton, () => OnClickSkinButton(_skinIndex, true));
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

    private void OnClickHairButton(int hairIndex, bool plus)
    {

    }

    private void OnClickSkinButton(int skinIndex, bool plus)
    {

    }

    private void SetIndex(ref int index, List<SkinnedMeshRenderer> meshList)
    {
        if (index < 0)
            index = meshList.Count - 1;           // 마지막 인덱스로 이동
        else if (index > meshList.Count - 1)
            index = 0;                            // 첫번째 인덱스로 이동
    }

    private void SetText(TextMeshProUGUI text, string defaultText, int index)
    {
        text.text = $"{defaultText}_{index + 1}";
    }
}
