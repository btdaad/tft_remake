using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TargetingStrategyBase", menuName = "Scriptable Objects/TargetingStrategyBase")]
public abstract class TargetingStrategyBase : ScriptableObject
{
    public abstract List<Unit> GetTargets(Unit caster);   
}
