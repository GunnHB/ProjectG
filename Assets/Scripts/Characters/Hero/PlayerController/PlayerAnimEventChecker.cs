using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimEventChecker : AnimEventChecker
{
    private bool _doCombo => GameManager.Instance.PController.DoCombo;

    protected override void StartAttack()
    {
        base.StartAttack();
    }

    protected override void EndAttack()
    {
        if (_doCombo)
            return;

        base.EndAttack();
    }

    protected override void StartCheckColliders()
    {
        base.StartCheckColliders();
    }

    protected override void EndCheckColliders()
    {
        base.EndCheckColliders();
    }

    protected override void ActiveGetHit()
    {
        base.ActiveGetHit();

        Debug.Log(_processingGetHit);
    }

    protected override void DeactiveGetHit()
    {
        base.DeactiveGetHit();

        Debug.Log(_processingGetHit);
    }
}
