using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AbilityBase", menuName = "Scriptable Objects/AbilityBase")]
public abstract class AbilityBase : ScriptableObject
{
    [SerializeField] public List<AbilityTargetBase> targetZones;
    public enum EffectType
    {
        HEALTH, // for healing purpose
        MAGIC_RESIST,
        PHYSICAL_DAMAGE,
        MAGIC_DAMAGE,
        STUN,
        NONE
    }
    public struct Effect
    {
        public float damage;
        public float duration;
        public EffectType stat;
        public Effect(float damage, EffectType stat, float duration = 0f)
        {
            this.damage = damage;
            this.duration = duration;
            this.stat = stat;
        }
    }
    public abstract List<List<Effect>> GetEffects(Unit caster);   
    protected float ScaleValueWithAP(Unit caster, float value)
    {
        Debug.Log($"[{Time.time}] ({caster.gameObject.name}): {value} scaled with AP of {caster.stats.abilityPower} + {caster.GetAP()}.");
        return value * (caster.stats.abilityPower + caster.GetAP()) / 100f;
    }
    protected float ScaleValueWithAD(Unit caster, float value)
    {
        Debug.Log($"[{Time.time}] ({caster.gameObject.name}): {value} scaled with AD of {caster.GetAD()}%.");
        return value + (value * caster.GetAD() / 100f);
    }
    protected float ScaleValueWithArmor(Unit caster, float value)
    {
        return (value * caster.GetArmor()) / 100f;
    }

    protected Effect GetMagicDamage(float damage, float duration = 0f)
    {
        return new Effect(damage, EffectType.MAGIC_DAMAGE, duration);
    }
    protected Effect GetPhysicalDamage(float damage, float duration = 0f)
    {
        return new Effect(damage, EffectType.PHYSICAL_DAMAGE, duration);
    }
}
