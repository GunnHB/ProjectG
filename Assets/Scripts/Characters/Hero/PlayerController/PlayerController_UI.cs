using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

using Sirenix.OdinInspector;

// UI 와 관련된 것들을 조작합니다.
public partial class PlayerController : MonoBehaviour
{
    private InputAction _inventoryAction;
    private InputAction _mainMenuAction;

    private InputAction _escapeAction;

    private void InventoryActionStarted(InputAction.CallbackContext context)
    {
        _input.SwitchCurrentActionMap("UI");
        GameManager.Instance.ChangeCurrentMode(GameManager.GameMode.Pause);

        ItemManager.Instance.OpenInventory();
    }

    private void MainMenuActionStarted(InputAction.CallbackContext context)
    {
        // 게임 진행 중일 때 실행됨
        if (GameManager.Instance.CurrentMode != GameManager.GameMode.InGame)
            return;

        _input.SwitchCurrentActionMap("UI");
        // GameManager.Instance.ChangeCurrentMode(GameManager.GameMode.Pause);
    }

    private void EscapeActionStarted(InputAction.CallbackContext context)
    {
        // 일시정지 (팝업이 열린 상태) 상태에서만 실행
        if (GameManager.Instance.CurrentMode != GameManager.GameMode.Pause)
            return;

        _input.SwitchCurrentActionMap("Player");

        UIManager.Instance.CloseUI(UIManager.Instance.FindOpenedPopupUI());
        GameManager.Instance.ChangeCurrentMode(GameManager.GameMode.InGame);
    }
}
