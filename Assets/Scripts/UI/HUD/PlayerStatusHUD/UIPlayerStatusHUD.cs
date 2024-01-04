using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

public class UIPlayerStatusHUD : UIHUDBase
{
    [Title("[Heart]")]
    [SerializeField] private ObjectPool _heartPool;

    private CharacterDataBase _playerDatabase;
    private List<HeartObject> _activedHeartList;    // 활성화된 하트 리스트

    private Coroutine _refreshHeartCoroutine;

    #region 캐싱
    private int _currPlayerIndex => GameManager.Instance.SelectedSlotIndex;
    #endregion

    protected override void Awake()
    {
        base.Awake();
    }

    // 하트의 1/4 은 5입니다.
    // 따라서 하트 하나 당 20임다.
    public void InitHeart(CharacterDataBase database)
    {
        _heartPool.Initialize();
        _heartPool.ReturnAllObject();

        _playerDatabase = database;

        if (_playerDatabase == null)
            return;

        SetPlayerHeart();
    }

    public void RefreshHeart(int damage)
    {
        if (_playerDatabase == null)
            return;

        // 데미지가 0 이하면 5로 고정시킴
        int actualDamage = damage <= 0 ? 5 : damage;

        if (_refreshHeartCoroutine != null)
        {
            StopCoroutine(_refreshHeartCoroutine);
            _refreshHeartCoroutine = null;
        }

        _refreshHeartCoroutine = StartCoroutine(nameof(Cor_RefreshHeart));
    }

    private IEnumerator Cor_RefreshHeart()
    {
        if (_activedHeartList.Count == 0)
            yield break;

        yield return null;
    }

    private void SetPlayerHeart()
    {
        // 최대 하트
        int maxHeartAmount = _playerDatabase.ThisMaxHP / (GameValue.QUATER_OF_HERAT_VLAUE * 4);

        // 현재 하트
        int currentHeartAmount = _playerDatabase.ThisCurrHP / (GameValue.QUATER_OF_HERAT_VLAUE * 4);
        // 현재 하트의 나머지
        int remainHeartAmount = _playerDatabase.ThisCurrHP % (GameValue.QUATER_OF_HERAT_VLAUE * 4);

        // 나머지가 있는지 없는지
        bool isRemain = remainHeartAmount != 0 ? true : false;
        // 나머지 중 켤 오브젝트의 개수
        int quaterCount = isRemain ? remainHeartAmount / GameValue.QUATER_OF_HERAT_VLAUE : 0;

        for (int index = 0; index < maxHeartAmount; index++)
        {
            var heart = _heartPool.GetObject();
            var heartObj = heart.GetComponent<HeartObject>();

            if (heartObj == null)
            {
                Debug.Log("there is no heart object! please check script.");
                break;
            }

            if (index < currentHeartAmount)
            {
                heartObj.SetHeart(4);
                _activedHeartList.Add(heartObj);
            }
            else if (isRemain)
            {
                heartObj.SetHeart(quaterCount);
                _activedHeartList.Add(heartObj);

                isRemain = false;
            }
        }
    }
}
