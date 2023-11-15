using GoogleSheet.Core.Type;

public static class GameValue
{
    // *** 해당 스크립트에 등록된 값들은 이변이 없는 한 변하지 않습니다. ***

    // 캐릭터 생성 시 초기값
    public static int INIT_HP = 250;
    public static int INIT_STAMINA = 250;
    public static int INIT_LEVEL = 1;

    // 게임에서 사용되는 기본적인 값
    public static float GRAVITY = -9.81f;
    public static int MAX_LEVEL = 50;

    public static float DECREASE_STAMINA_VALUE = 1f;
    public static float INCREASE_STAMINA_VALUE = 2f;
    public static float CHARGE_STAMINA_TIME = 5f;

    // 저장 공간
    public static int SAVE_SLOT_COUNT = System.Enum.GetValues(typeof(SlotIndex)).Length;
}

public enum SlotIndex
{
    First,
    Second,
    Third,
}

// 테이블에 사용되는 enum 은 요기에 추가합시다.
#region 
// 해당 어트리뷰트를 추가해줘야 테이블에서 enum 으로 인식합니다.
[UGS(typeof(ItemType))]
public enum ItemType
{
    Armor,
    Weapon,
    Food,
    Default,
}

[UGS(typeof(ArmorType))]
public enum ArmorType
{
    Helmet,
    Crown,
    Hat,
    Closthes,
    Pauldrons,
    Belt,
    Gloves,
    Boots,
}

[UGS(typeof(WeaponType))]
public enum WeaponType
{
    OneHand,    // 한손 무기
    TwoHand,    // 양손 무기
    Shield,     // 방패 (방패가 무기로 들어가도 되나..???)
    Bow,        // 활
}
#endregion