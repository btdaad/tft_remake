using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AbilityBase", menuName = "Scriptable Objects/AbilityBase")]
public abstract class AbilityBase : ScriptableObject
{
    [SerializeField] public AbilityTargetBase targetZone;
    public enum EffectType
    {
        HEALTH, // for healing purpose
        MAGIC_RESIST,
        PHYSICAL_DAMAGE,
        MAGIC_DAMAGE,
        NONE
    }
    public struct Effect
    {
        public float damage;
        public EffectType stat;
        public Effect(float damage, EffectType stat)
        {
            this.damage = damage;
            this.stat = stat;
        }
    }
    public abstract List<Effect> GetEffect(Unit caster);   
    protected Effect GetMagicDamage(Unit caster, float damage)
    {
        float magicDamage = damage * (caster.stats.abilityPower + caster.GetAD()) / 100f;
        return new Effect(magicDamage, EffectType.MAGIC_DAMAGE);
    }
    protected Effect GetPhysicalDamage(Unit caster, float damage)
    {
        float physicalDamage = damage * (caster.stats.attackDamage[(int) caster.stats.star] + caster.GetAP()) / 100f;
        return new Effect(physicalDamage, EffectType.PHYSICAL_DAMAGE);
    }
}
