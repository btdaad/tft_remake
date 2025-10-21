using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AbilityBase", menuName = "Scriptable Objects/AbilityBase")]
public abstract class AbilityBase : ScriptableObject
{
    [SerializeField] private List<Unit> _targets;
    public abstract void Cast(Unit caster);   
}
