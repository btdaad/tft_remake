using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Targeting/Line")]
public class Line : AbilityTargetBase
{
    public override List<Unit> GetTargets(Unit caster)
    {
        // get unit position
        // get target direction

        return new List<Unit>(); // TODO
    } 
}
