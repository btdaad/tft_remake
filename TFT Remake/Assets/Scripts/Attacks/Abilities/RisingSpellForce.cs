using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Abilities/Ezreal's")]

/* Fire a blast at the current target, dealing 230/350/600 (AP) magic damage and 115/175/300 (AD) physical damage. */
public class RisingSpellForce : AbilityBase
{
    private int[] damageAP = { 230, 350, 600 }; // 230/350/600
    private int[] damageAD = { 115, 175, 300 }; // 115/175/300
    public override List<Effect> GetEffect(Unit caster)
    {
        List<Effect> effects = new List<Effect>();
        float magicDamage = ScaleValueWithAP(caster, damageAP[(int)caster.stats.star]);
        effects.Add(GetMagicDamage(magicDamage));
        float physicalDamage = ScaleValueWithAD(caster, damageAD[(int)caster.stats.star]);
        effects.Add(GetPhysicalDamage(physicalDamage));
        return effects;
    }
}