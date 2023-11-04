using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CustomizeHUD : UIHUDBase
{
    [Title("Buttons")]
    [TabGroup("LeftPanel"), SerializeField]
    private Button _leftRotateButton;
    [TabGroup("LeftPanel"), SerializeField]
    private Button _resetRotateButton;
    [TabGroup("LeftPanel"), SerializeField]
    private Button _rightRotateButton;
    [Title("Options")]
    [TabGroup("LeftPanel"), SerializeField]
    private GameObject _characterObj;
    [TabGroup("LeftPanel"), SerializeField, Range(1f, 10f)]
    private float _rotateSpeed = 7f;

    [Title("Hair")]
    [TabGroup("RightPanel"), SerializeField] private Button _hairLeftButton;
    [TabGroup("RightPanel"), SerializeField] private Button _hairRightButton;
    [TabGroup("RightPanel"), SerializeField] private TextMeshProUGUI _hairInfoText;
    [TabGroup("RightPanel"), SerializeField] private List<SkinnedMeshRenderer> _hairMeshList = new();

    [Title("Skin")]
    [TabGroup("RightPanel"), SerializeField] private Button _skinLeftButton;
    [TabGroup("RightPanel"), SerializeField] private Button _skinRightButton;
    [TabGroup("RightPanel"), SerializeField] private TextMeshProUGUI _skinInfoText;
    [TabGroup("RightPanel"), SerializeField] private List<SkinnedMeshRenderer> _skinMeshList = new();

    // 메시 선택 변수
    private int _hairIndex = 0;
    private int _skinIndex = 0;

    // 캐릭터 회전 관련 변수
    private Quaternion _originRotate;           // 기본 로테이션
    private Coroutine _rotateCoroutine;         // 회전 코루틴
    private bool _reverse = false;              // 위에서 봤을 때 시계 방향이 true // 기본적으로 true
    private bool _isPress = false;              // 버튼 눌리는 중인지
    private float _resetSpeed = 10f;            // 회전 리셋 속도 (빠르게 원위치)

    protected override void Start()
    {
        base.Start();

        SetLeftButton();
        SetRightButton();
        SetInit();
    }

    private void SetLeftButton()
    {
        if (_characterObj != null)
            _originRotate = _characterObj.transform.rotation;

        Util.AddButtonListener(_leftRotateButton, EventTriggerType.PointerDown, (data) =>
        {
            _reverse = true;
            PointerDownRotateButton(data);
        });

        Util.AddButtonListener(_rightRotateButton, EventTriggerType.PointerDown, (data) =>
        {
            _reverse = false;
            PointerDownRotateButton(data);
        });

        Util.AddButtonListener(_leftRotateButton, EventTriggerType.PointerUp, PointerUpRotateButton);
        Util.AddButtonListener(_rightRotateButton, EventTriggerType.PointerUp, PointerUpRotateButton);

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

    private void SetInit()
    {
        // 일단 다 끄기
        foreach (var hair in _hairMeshList)
            hair.gameObject.SetActive(false);

        foreach (var skin in _skinMeshList)
            skin.gameObject.SetActive(false);

        _hairMeshList[_hairIndex].gameObject.SetActive(true);
        _skinMeshList[_skinIndex].gameObject.SetActive(true);
    }

    private void PointerDownRotateButton(BaseEventData data)
    {
        _isPress = true;

        if (_rotateCoroutine != null)
        {
            StopCoroutine(_rotateCoroutine);
            _rotateCoroutine = null;
        }

        _rotateCoroutine = StartCoroutine(nameof(Cor_Rotate));
    }

    private void PointerUpRotateButton(BaseEventData data)
    {
        _reverse = true;
        _isPress = false;

        if (_rotateCoroutine != null)
        {
            StopCoroutine(_rotateCoroutine);
            _rotateCoroutine = null;
        }
    }

    private void OnClickResetRotateButton()
    {
        // 계산해서 가까운 방향으로 회전시키기

        // 기존 y 값에서 현재 y 걊을 빼기
        float angle = _originRotate.eulerAngles.y - _characterObj.transform.rotation.eulerAngles.y;

        // 현재 각도는 반시계 방향과 가까운 회전한 상태
        _reverse = angle < 0;

        if (_rotateCoroutine != null)
        {
            StopCoroutine(_rotateCoroutine);
            _rotateCoroutine = null;
        }

        _rotateCoroutine = StartCoroutine(nameof(Cor_RotateReset));
    }

    private IEnumerator Cor_Rotate()
    {
        int multiValue = _reverse ? 1 : -1;

        while (_isPress)
        {
            _characterObj.transform.Rotate(Vector3.up * _rotateSpeed * multiValue * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator Cor_RotateReset()
    {
        // 현재 보고있는 방향에서 원점에 가까운 쪽으로 회전해야함
        int multiValue = _reverse ? -1 : 1;
        int parsingValue = (int)_characterObj.transform.rotation.eulerAngles.y;

        yield break;
    }

    private void OnClickHairButton(int hairIndex, bool plus)
    {
        _hairMeshList[_hairIndex].gameObject.SetActive(false);

        hairIndex = plus ? hairIndex++ : hairIndex--;

        SetIndex(ref _hairIndex, _hairMeshList);
        SetText(_hairInfoText, "Hair", _hairIndex);

        _hairMeshList[_hairIndex].gameObject.SetActive(true);
    }

    private void OnClickSkinButton(int skinIndex, bool plus)
    {
        _skinMeshList[_skinIndex].gameObject.SetActive(false);

        skinIndex = plus ? skinIndex++ : skinIndex--;

        SetIndex(ref _skinIndex, _skinMeshList);
        SetText(_skinInfoText, "Skin", _skinIndex);

        _skinMeshList[_skinIndex].gameObject.SetActive(true);
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
