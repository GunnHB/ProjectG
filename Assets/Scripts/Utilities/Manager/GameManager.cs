using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class GameManager : SingletonObject<GameManager>
{
    public enum GameMode
    {
        None,
        Title,
        InGame,
        Pause,
    }

    private GameMode _currentMode = GameMode.None;
    public GameMode CurrentMode => _currentMode;

    private int _selectedSlotIndex;
    public int SelectedSlotIndex => _selectedSlotIndex;

    private PlayerController _pController;
    public PlayerController PController
    {
        get
        {
            if (_currentMode == GameMode.InGame)
                _pController = GameObject.Find("Player").GetComponent<PlayerController>();

            return _pController;
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public void ChangeCurrentMode(GameMode changeMode)
    {
        _currentMode = changeMode;
    }

    public void SetSelectedSlotIndex(int index)
    {
        _selectedSlotIndex = index;
    }
}
