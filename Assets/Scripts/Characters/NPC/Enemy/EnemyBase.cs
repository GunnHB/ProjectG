using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;
using System.Linq;

public class EnemyBase : CharacterBase
{
    private const string GROUP_SET_DATA = "[SET DATA]";
    private const string TITLE_DATABASE = "[ DataBase ]";
    private const string TITLE_AI = "[ AI ]";

    public const string ANIM_PARAM_ALERT = "IsAlert";

    [Title(TITLE_DATABASE), ValueDropdown(nameof(EnemyList)), HideLabel]
    [OnValueChanged(nameof(SetData)), SerializeField]
    private string _enemy;

    [SerializeField]
    private EnemyDataBase _enemyDatabase;

    [Title(TITLE_AI), SerializeField]
    private EnemyBehaviorBase _enemyAI;

    // <프리팹명, 아이디>
    private Dictionary<string, long> _enemyDic = new();

    // 프로퍼티
    public EnemyDataBase Database => _enemyDatabase;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    private List<string> EnemyList
    {
        get
        {
            Enemy.Data.Load();

            _enemyDic.Clear();

            foreach (var enemy in Enemy.Data.DataList)
                _enemyDic.Add(enemy.prefab_name, enemy.id);

            return _enemyDic.Keys.ToList();
        }
    }

    private void SetData()
    {
        if (string.IsNullOrEmpty(_enemy))
            return;

        long enemyId = _enemyDic[_enemy];

        if (enemyId == 0)
            return;

        var data = Enemy.Data.DataMap[enemyId];

        // info
        _enemyDatabase.ThisName = data.name;
        _enemyDatabase.ThisDescription = data.desc;

        // status
        _enemyDatabase.ThisMaxHP = data.hp;
        _enemyDatabase.ThisCurrHP = data.hp;

        // power
        _enemyDatabase.ThisOffensivePower = data.offensive_power;
        _enemyDatabase.ThisDefensivePower = data.defensive_power;

        // speed
        _enemyDatabase.ThisWalkSpeed = data.walk_speed;
        _enemyDatabase.ThisSprintSpeed = data.sprint_speed;

        // // drop item 아이템 쪽 리팩토링 이후 추가 예정
        // _enemyDatabase.DropItemList.Add(new ItemData() {});
    }
}
