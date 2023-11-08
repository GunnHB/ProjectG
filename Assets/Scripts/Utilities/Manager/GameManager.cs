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
    }

    private GameMode _currentMode = GameMode.None;
    public GameMode CurrentMode => _currentMode;

    private int _selectedSlotIndex;
    public int SelectedSlotIndex => _selectedSlotIndex;

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
