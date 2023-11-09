using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Sirenix.OdinInspector;

public class UITitlePanel : UIPanelBase
{
    [Title("Title")]
    [SerializeField] private TextMeshProUGUI _pressAnyKeyText;

    [Title("Buttons")]
    [SerializeField] private GameObject _buttonGroupObj;
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _exitGameButton;

    private bool _isPressAnyKey;

    private Coroutine _anyKeyCoroutine;

    protected override void Awake()
    {
        base.Awake();

        Util.AddButtonListener(_startGameButton, OnClickStartGameButton);
        Util.AddButtonListener(_settingsButton, OnClickSettingsButton);
        Util.AddButtonListener(_exitGameButton, OnClickExitGameButton);
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        _isPressAnyKey = false;

        _buttonGroupObj.SetActive(false);
        _pressAnyKeyText.gameObject.SetActive(true);

        if (_anyKeyCoroutine != null)
        {
            StopCoroutine(_anyKeyCoroutine);
            _anyKeyCoroutine = null;
        }

        _anyKeyCoroutine = StartCoroutine(nameof(Cor_PressAnyKey));

        GameManager.Instance.ChangeCurrentMode(GameManager.GameMode.Title);
    }

    private IEnumerator Cor_PressAnyKey()
    {
        while (!_isPressAnyKey)
        {
            if (Input.anyKey)
            {
                // 활성화되면 잠시 대기
                yield return new WaitForSeconds(1f);
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

    private void OnClickStartGameButton()
    {
        LoadSceneManager.Instance.FadeInOut(OpenSelectCharacterPanel);
    }

    private void OpenSelectCharacterPanel()
    {
        // 타이틀 패널은 숨김
        this.gameObject.SetActive(false);

        var openedUI = UIManager.Instance.FindOpendUI<UISelectCharacterPanel>(UIManager.Instance.PanelCanvas);

        if (openedUI != null)
            openedUI.gameObject.SetActive(true);
        else
            openedUI = UIManager.Instance.OpenUI<UISelectCharacterPanel>("SelectCharacterPanel/UISelectCharacterPanel");

        openedUI.Init();
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

    private void OpenSettingPopup()
    {

    }
}
