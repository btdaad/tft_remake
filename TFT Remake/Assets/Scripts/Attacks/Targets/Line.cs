using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Targeting/Line")]
public class Line : AbilityTargetBase
{
    [SerializeField] LayerMask unitMask;
    List<Unit> _targets = new List<Unit>();
    public override void SetTarget(Transform target)
    {
        _target = target;
    }

    public override List<Unit> GetTargets(Unit caster)
    {
        bool isFromPlayerTeam = caster.IsFromPlayerTeam();

        Vector3 dir = _target.position - caster.transform.position;
        RaycastHit[] hits = Physics.RaycastAll(caster.transform.position, dir, 10.0f, unitMask);

        List<Unit> targets = new List<Unit>();

        foreach (var hit in hits)
        {
            Unit unit = hit.collider.GetComponent<Unit>();
            if (unit != null && unit.IsFromPlayerTeam() != isFromPlayerTeam)
                targets.Add(unit);
        }

        return targets;
    }
}