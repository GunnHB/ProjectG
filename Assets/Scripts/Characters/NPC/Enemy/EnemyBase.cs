using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

public class EnemyBase : CharacterBase
{
    private const string TITLE_DATABASE = "[ DataBase ]";
    private const string TITLE_AI = "[ AI ]";

    public const string ANIM_PARAM_ALERT = "IsAlert";

    [Title(TITLE_DATABASE), SerializeField]
    private EnemyDataBase _enemyDatabase;

    [Title(TITLE_AI), SerializeField]
    private EnemyAIV2 _enemyAI;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
