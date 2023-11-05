
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Sirenix.OdinInspector;

using DG.Tweening;

public class UITitlePanel : MonoBehaviour
{
    [Title("Title")]
    [SerializeField] private TextMeshProUGUI _pressAnyKeyText;

    [Title("Buttons")]
    [SerializeField] private GameObject _buttonGroupObj;
    [SerializeField] private Button _newGameButton;
    [SerializeField] private Button _loadGameButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _exitGameButton;

    private bool _isPressAnyKey;

    private Coroutine _testCoroutine = null;

    private void Awake()
    {
        Util.AddButtonListener(_newGameButton, OnClickNewGameButton);
        Util.AddButtonListener(_loadGameButton, OnClickLoadGameButton);
        Util.AddButtonListener(_settingsButton, OnClickSettingsButton);
        Util.AddButtonListener(_exitGameButton, OnClickExitGameButton);
    }

    private void Start()
    {
        StopAllCoroutines();
        StartCoroutine(nameof(PressAnyKeyCoroutine));
    }

    private IEnumerator PressAnyKeyCoroutine()
    {
        while (!_isPressAnyKey)
        {
            if (Input.anyKey)
            {
                // 활성화되면 잠시 대기
                yield return new WaitForSeconds(2f);
                _isPressAnyKey = true;
            }

            if (_isPressAnyKey)
            {
                _pressAnyKeyText.gameObject.SetActive(false);

                yield return new WaitForSeconds(.5f);

                _buttonGroupObj.SetActive(true);
            }

            yield return null;
        }
    }

    private void OnClickNewGameButton()
    {
        // 새 게임 생성
        string titleString = "NOTICE";
        string msgString = "Create new game?";

        UIManager.Instance.OpenCommonDialogue(titleString, msgString, CreateNewGame, null);
    }

    private void OnClickLoadGameButton()
    {
        // 게임 불러오기
    }

    private void OnClickSettingsButton()
    {
        // 게임 설정
    }

    private void OnClickExitGameButton()
    {
        // 게임 종료
        Application.Quit();
    }

    private void CreateNewGame()
    {
        // Debug.Log("새 게임!");

        if (_testCoroutine != null)
        {
            StopCoroutine(_testCoroutine);
            _testCoroutine = null;
        }

        _testCoroutine = StartCoroutine(nameof(Cor_Test));
    }

    private IEnumerator Cor_Test()
    {
        while (true)
        {
            Debug.Log("TEST");

            yield return new WaitForSeconds(1f);
        }
    }
}
