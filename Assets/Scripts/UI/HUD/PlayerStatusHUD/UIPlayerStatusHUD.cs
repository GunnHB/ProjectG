using UnityEngine;

using Sirenix.OdinInspector;

public class UIPlayerStatusHUD : UIHUDBase
{
    [Title("[Heart]")]
    [SerializeField] private ObjectPool _heartPool;

    #region 캐싱
    private int _currPlayerIndex => GameManager.Instance.SelectedSlotIndex;

    private int _currPlayerHP => JsonManager.Instance.BaseData._playerHP[_currPlayerIndex];
    private int _currPlayerStamina => JsonManager.Instance.BaseData._playerStamina[_currPlayerIndex];
    #endregion

    protected override void Awake()
    {
        base.Awake();

        InitHeart();
    }

    // 하트의 1/4 은 5입니다.
    // 따라서 하트 하나 당 20임다.
    private void InitHeart()
    {
        _heartPool.Initialize();
        _heartPool.ReturnAllObject();

        if (_currPlayerHP == 0)
            return;

        CalcPlayerHP();
    }

    private void CalcPlayerHP()
    {
        int wholeHeartAmount = _currPlayerHP / (GameValue.QUATER_OF_HERAT_VLAUE * 4);
        int resultHeart = (_currPlayerHP % (GameValue.QUATER_OF_HERAT_VLAUE * 4));
        int quaterCount = 0;

        // 완전한 하트를 채울 정도가 아닌 체력이 있는 경우
        if (resultHeart != 0)
        {
            quaterCount = resultHeart / GameValue.QUATER_OF_HERAT_VLAUE;
            wholeHeartAmount += 1;  // 나머지도 표시해야하기 때문에 1 추가
        }

        for (int index = 0; index < wholeHeartAmount; index++)
        {
            var heartObj = _heartPool.GetObject();
        }
    }
}
