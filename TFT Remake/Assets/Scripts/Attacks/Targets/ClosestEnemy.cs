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

// [CreateAssetMenu(menuName = "Targeting/Farthest Enemy")]
// public class FarthestEnemy : AbilityTargetBase 
// {
//     public override List<Unit> GetTargets(Unit caster)
//     {
//         return new List<Unit>(); // TODO
//     } 
// }