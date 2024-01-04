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
                _pController = GameObject.Find("Player_onCamera").GetComponent<PlayerController>();

            return _pController;
        }
    }

    #region 캐싱
    private Mesh _hairMesh { get => JsonManager.Instance.MeshData._hairHesh[_selectedSlotIndex]; }
    private Mesh _skinMesh { get => JsonManager.Instance.MeshData._skinMesh[_selectedSlotIndex]; }

    public string PlayerName => JsonManager.Instance.BaseData._playerName[_selectedSlotIndex];

    public int PlayerMaxHP => JsonManager.Instance.BaseData._playerMaxHP[_selectedSlotIndex];
    public int PlayerCurrentHP = 35;//=> JsonManager.Instance.BaseData._playerCurrentHP[_selectedSlotIndex];

    public float PlayerStamina => JsonManager.Instance.BaseData._playerStamina[_selectedSlotIndex];
    #endregion

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

    public void LoadPlayerCharacter()
    {
        if (LoadSceneManager.Instance.CurrentSceneType != LoadSceneManager.SceneType.InGame)
            return;

        ChangeCurrentMode(GameMode.InGame);

        var playerPrefab = ResourceManager.Instance.GetPlayerPrefab<GameObject>(GameValue.PLAYER_PREFAB);

        if (playerPrefab == null)
        {
            Debug.LogWarning("there is no player!");
            return;
        }

        var player = Instantiate(playerPrefab);

        if (player == null)
            return;

        if (player.TryGetComponent(out PlayerSkinnedMesh skinnedMeshInfo))
        {
            skinnedMeshInfo.SetPlayerSkinnedMesh(PlayerSkinnedMesh.SKINNED_MESH_HAIR, _hairMesh);
            skinnedMeshInfo.SetPlayerSkinnedMesh(PlayerSkinnedMesh.SKINNED_MESH_SKIN, _skinMesh);
        }
    }
}
