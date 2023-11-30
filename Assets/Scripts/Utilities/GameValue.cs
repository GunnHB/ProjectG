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

    // 초기 인벤토리 용량 (캐릭터 생성 시에만 사용합니다.)
    public static int INVENTORY_DEFAULT_CATE_WEAPON_SIZE = 4;
    public static int INVENTORY_DEFAULT_CATE_ARMOR_SIZE = 10;
    public static int INVENTORY_DEFAULT_CATE_SHIELD_SIZE = 4;
    public static int INVENTORY_DEFAULT_CATE_BOW_SIZE = 4;
    public static int INVENTORY_DEFAULT_CATE_FOOD_SIZE = 30;
    public static int INVENTORY_DEFAULT_CATE_DEFAULT_SIZE = 30;

    // 인벤토리 한 행에 들어가는 슬롯 수
    public static int INVENTORY_ROW_AMOUNT = 5;

    // 플레이어 애니 관련
    public static string PLAYER_ANIM_ATTACK_01 = "Attack_01";
    public static string PLAYER_ANIM_ATTACK_02 = "Attack_02";
    public static string PLAYER_ANIM_ATTACK_03 = "Attack_03";
    public static string PLAYER_ANIM_ATTACK_04 = "Attack_04";
    public static string PLAYER_ANIM_ATTACK_05 = "Attack_05";
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
    // 한손무기 / 화살은 오른손으로 들어감다.
    // 나머지는 왼손으로 들어감다.
    OneHand,    // 한손 무기
    TwoHand,    // 양손 무기
    Shield,     // 방패 (방패가 무기로 들어가도 되나..???)
    Bow,        // 활
    Arrow,      // 화살
}

[UGS(typeof(InventoryCategory))]
public enum InventoryCategory
{
    CategoryWeapon,
    CategoryArmor,
    CategoryShield,
    CategoryBow,
    CategoryFood,
    CategoryDefault,
}

[UGS(typeof(StatType))]
public enum StatType
{
    None = -1,
    Attack,
    Defend,
}
#endregion