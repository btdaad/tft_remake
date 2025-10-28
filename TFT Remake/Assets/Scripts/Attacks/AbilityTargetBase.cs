using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AbilityTargetBase", menuName = "Scriptable Objects/AbilityTargetBase")]
public abstract class AbilityTargetBase : ScriptableObject
{
    protected Transform _target;
    public abstract void SetTarget(Transform target);
    public abstract List<Unit> GetTargets(Unit caster);   
}
