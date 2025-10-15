using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Targeting/Closest Enemy")]
public class ClosestEnemy : TargetingStrategyBase
{
    public override List<Unit> GetTargets(Unit caster)
    {
        return new List<Unit>();
    } 
}

[CreateAssetMenu(menuName = "Targeting/AoE")]
public class AoE : TargetingStrategyBase
{
    [SerializeField] int radius;
    public override List<Unit> GetTargets(Unit caster)
    {
        return new List<Unit>();
    } 
}

[CreateAssetMenu(menuName = "Targeting/Cone")]
public class Cone : TargetingStrategyBase
{
    [SerializeField] int radius;
    public override List<Unit> GetTargets(Unit caster)
    {
        return new List<Unit>();
    } 
}

[CreateAssetMenu(menuName = "Targeting/Line")]
public class Line : TargetingStrategyBase
{
    public override List<Unit> GetTargets(Unit caster)
    {
        return new List<Unit>();
    } 
}
