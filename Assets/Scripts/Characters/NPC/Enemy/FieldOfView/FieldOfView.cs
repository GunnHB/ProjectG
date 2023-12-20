using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

public class FieldOfView : MonoBehaviour
{
    private const string TITLE_RANGE = "[Range]";
    private const string TITLE_LAYER_MASK = "[LayerMask]";

    [TitleGroup(TITLE_RANGE), Range(0, 15)]
    [SerializeField] private float _viewRadius = 5f;
    [TitleGroup(TITLE_RANGE), Range(0, 360)]
    [SerializeField] private float _viewAngle = 90f;
    [TitleGroup(TITLE_RANGE), Range(0f, 3f)]
    [SerializeField] private float _meleeAttackRange;
    [SerializeField] private bool _canRangeAttack;
    [TitleGroup(TITLE_RANGE), Range(0f, 7f), ShowIf(nameof(_canRangeAttack))]
    [SerializeField] private float _rangeAttackRange;

    [TitleGroup(TITLE_LAYER_MASK)]
    [SerializeField] private LayerMask _targetMask;
    [TitleGroup(TITLE_LAYER_MASK)]
    [SerializeField] private LayerMask _obstacleMask;

    private List<Transform> _visibleTargetList = new List<Transform>();
    private Transform _nearestTarget;

    private float _distanceToTarget = 0f;

    public float ViewAngle => _viewAngle;
    public float ViewRadius => _viewRadius;
    public float MeleeAttackRange => _meleeAttackRange;
    public float RangeAttackRange => _rangeAttackRange;

    public bool CanRangeAttack => _canRangeAttack;

    public List<Transform> VisibleTargetList => _visibleTargetList;
    public Transform NearestTarget => _nearestTarget;

    public LayerMask TargetMask => _targetMask;
    public LayerMask ObstacleMask => _obstacleMask;

    public void FindVisibleTargets()
    {
        _distanceToTarget = 0f;
        _nearestTarget = null;
        _visibleTargetList.Clear();

        // 범위 내의 모든 콜라이더 가져옴
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, _viewRadius, _targetMask);

        if (targetsInViewRadius.Length != 0)
        {
            foreach (Collider collider in targetsInViewRadius)
            {
                if (collider.TryGetComponent(out Transform targetTransform))
                {
                    // 타켓의 방향
                    Vector3 dirToTarget = (targetTransform.position - transform.position).normalized;

                    // transform.forward 에서 타켓의 방향까지의 각도가 _viewAngle 의 절반인지 확인
                    if (Vector3.Angle(transform.forward, dirToTarget) < _viewAngle / 2)
                    {
                        float dstToTarget = Vector3.Distance(transform.position, targetTransform.position);

                        // 자신과 타켓 사이에 장애물이 있는지 확인
                        if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, _obstacleMask))
                        {
                            _visibleTargetList.Add(targetTransform);

                            // 가장 가까운 타겟이 없거나 이전 타겟까지의 거리가 새롭게 추가한 타겟보다 멀다면
                            if (_nearestTarget == null || _distanceToTarget > dstToTarget)
                            {
                                _nearestTarget = targetTransform;
                                _distanceToTarget = dstToTarget;
                            }
                        }
                    }
                }
            }
        }
    }

    // 시야각의 방향 구하기
    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
            angleInDegrees += transform.eulerAngles.y;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
