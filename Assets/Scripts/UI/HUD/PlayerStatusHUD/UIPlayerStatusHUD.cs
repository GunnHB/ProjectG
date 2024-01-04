using UnityEngine;

using Sirenix.OdinInspector;

public class UIPlayerStatusHUD : UIHUDBase
{
    [Title("[Heart]")]
    [SerializeField] private ObjectPool _heartPool;

    private CharacterDataBase _playerDatabase;

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

        CalcPlayerHP();
    }

    public void RefreshHeart()
    {
        Debug.Log("decresse heart");
    }

    private void CalcPlayerHP()
    {
        // 완전한 하트 개수
        int wholeHeartAmount = _playerDatabase.ThisMaxHP / (GameValue.QUATER_OF_HERAT_VLAUE * 4);
        // 불완전한 하트 개수
        int remainHeart = _playerDatabase.ThisMaxHP % (GameValue.QUATER_OF_HERAT_VLAUE * 4);
        // 쿼터 개수
        int quaterCount = 0;

        bool remain = false;

        // 완전한 하트를 채울 정도가 아닌 체력이 있는 경우
        if (remainHeart != 0)
        {
            quaterCount = remainHeart / GameValue.QUATER_OF_HERAT_VLAUE;
            wholeHeartAmount += 1;  // 나머지도 표시해야하기 때문에 1 추가

            remain = true;
        }

        for (int index = 0; index < wholeHeartAmount; index++)
        {
            var heart = _heartPool.GetObject();
            var heartObj = heart.GetComponent<HeartObject>();

            if (heartObj == null)
                return;

            // 나머지가 있는 경우 마지막은 나머지의 값 만큼만 표기 
            if (index == wholeHeartAmount - 1 && remain)
            {
                float heartAmount = 1f / 4f * quaterCount;

                heartObj.SetHeart(heartAmount);
            }
            else
                heartObj.SetHeart(1f);
        }
    }
}
