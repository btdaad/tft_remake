using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AbilityBase", menuName = "Scriptable Objects/AbilityBase")]
public abstract class AbilityBase : ScriptableObject
{
    [SerializeField] public AbilityTargetBase targetZone;
    public struct Effect
    {
        public float damage;
        public bool isPhysicalDamage;
        public Effect(float damage, bool isPhysicalDamage)
        {
            this.damage = damage;
            this.isPhysicalDamage = isPhysicalDamage;
        }
    }
    public abstract Effect GetDamage(Unit caster);   
}
