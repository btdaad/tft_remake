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
    protected float ScaleValueWithAP(Unit caster, float value)
    {
        return value * (caster.stats.abilityPower + caster.GetAD()) / 100f;
    }
    protected float ScaleValueWithAD(Unit caster, float value)
    {
        return value * (caster.stats.attackDamage[(int) caster.stats.star] + caster.GetAP()) / 100f;
    }

    protected Effect GetMagicDamage(float damage)
    {
        return new Effect(damage, EffectType.MAGIC_DAMAGE);
    }
    protected Effect GetPhysicalDamage(float damage)
    {
        return new Effect(damage, EffectType.PHYSICAL_DAMAGE);
    }
}
