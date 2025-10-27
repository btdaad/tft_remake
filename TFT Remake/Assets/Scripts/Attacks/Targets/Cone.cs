using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Targeting/Cone")]
public class Cone : AbilityTargetBase
{
    [SerializeField] int radius;
    public override List<Unit> GetTargets(Unit caster)
    {
        // get unit position
        // get corresponding cell
        // get unit closest enemy
        // make a cone formula in the direction of the closest enemy
        return new List<Unit>(); // TODO
    } 
}