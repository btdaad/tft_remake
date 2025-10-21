using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AbilityTargetBase", menuName = "Scriptable Objects/AbilityTargetBase")]
public abstract class AbilityTargetBase : ScriptableObject
{
    public abstract List<Unit> GetTargets(Unit caster);   
}
