using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimEventChecker : AnimEventChecker
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void StartAttack()
    {
        base.StartAttack();
    }

    protected override void EndAttack()
    {
        base.EndAttack();
    }

    protected override void StartCheckColliders()
    {
        base.StartCheckColliders();

        if (_collider.TryGetComponent(out PlayerController pController))
        {
            pController.GetDamaged(5);
        }
    }

    protected override void EndCheckColliders()
    {
        base.EndCheckColliders();
    }
}
