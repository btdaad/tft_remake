using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Targeting/Closest Enemy")]
public class ClosestEnemy : AbilityTargetBase 
{
    public override void SetTarget(Transform target)
    {
        _target = target;
    }
    public override List<Unit> GetTargets(Unit caster)
    {
        List<Unit> targets = new List<Unit>();
        targets.Add(_target.GetComponent<Unit>());
        return targets;
    } 
}