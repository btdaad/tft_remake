using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Targeting/Closest Enemy")]
public class ClosestEnemy : AbilityTargetBase 
{
    public override List<Unit> GetTargets(Unit caster)
    {
        return null;
    } 
}
[CreateAssetMenu(menuName = "Targeting/Farthest Enemy")]
public class FarthestEnemy : AbilityTargetBase 
{
    public override List<Unit> GetTargets(Unit caster)
    {
        return new List<Unit>(); // TODO
    } 
}

[CreateAssetMenu(menuName = "Targeting/AoE")]
public class AoE : AbilityTargetBase
{
    [SerializeField] int radius;
    public override List<Unit> GetTargets(Unit caster)
    {
        return new List<Unit>(); // TODO
    } 
}

[CreateAssetMenu(menuName = "Targeting/Cone")]
public class Cone : AbilityTargetBase
{
    [SerializeField] int radius;
    public override List<Unit> GetTargets(Unit caster)
    {
        return new List<Unit>(); // TODO
    } 
}

[CreateAssetMenu(menuName = "Targeting/Line")]
public class Line : AbilityTargetBase
{
    public override List<Unit> GetTargets(Unit caster)
    {
        return new List<Unit>(); // TODO
    } 
}
