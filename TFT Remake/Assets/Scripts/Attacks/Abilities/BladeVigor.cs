using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Abilities/Aatrox's")]

/* Heal 126/152/194 (AP/Health) and deal 125/190/280 (AD/AP) physical damage to the current target.
   Healing and damage are increased by up to 100% based on Aatrox's missing health.

   Heal: 126/152/194 = 4% Health + 100/105/110 (AP)
   Damage: 125/190/280 = 100/150/225 (AD) + 25/40/55 (AP) */
public class BladeVigor : AbilityBase
{
    private int[] healAP = { 100, 105, 110 };
    private float healHealth = 0.04f;
    private int[] damageAD = { 100, 150, 250 };
    private int[] damageAP = { 25, 40, 55 };
    
    public override List<Effect> GetEffect(Unit caster)
    {
        float missingHealth = - (caster.GetHealth() - caster.GetMaxHealth());
        float missingHealthRatio = missingHealth / caster.GetMaxHealth();
        float missingHealthPercent = Mathf.Lerp(0, 1, missingHealthRatio);

        List<Effect> effects = new List<Effect>();

        float heal = ScaleValueWithAP(caster, healAP[(int)caster.stats.star]) + healHealth * caster.GetMaxHealth();
        heal += missingHealthPercent * heal;
        effects.Add(new Effect(heal, EffectType.HEALTH));

        float physicalDamageAD = ScaleValueWithAD(caster, damageAD[(int)caster.stats.star]);
        float physicalDamageAP = ScaleValueWithAP(caster, damageAP[(int)caster.stats.star]);
        float physicalDamage = physicalDamageAD + physicalDamageAP;
        physicalDamage += missingHealthPercent * physicalDamage;
        effects.Add(GetPhysicalDamage(physicalDamage));

        return effects;
    }
}