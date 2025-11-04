using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Abilities/Jayce's")]

/* Deal 200/300/475 (AD/AP) physical damage to enemies in a 1-hex radius centered on the target. 

   Damage: 200/300/475 = 180/270/425 (AD) + 20/30/50 (AP) */
public class ThunderingHammerstrike : AbilityBase
{
    private int[] damageAD = { 180, 270, 425 };
    private int[] damageAP = { 20, 30, 50 };
    
    public override List<List<Effect>> GetEffects(Unit caster)
    {
        List<List<Effect>> listEffects = new List<List<Effect>>();

        List<Effect> effects = new List<Effect>();
        float damageAD = ScaleValueWithAD(caster, this.damageAD[(int)caster.stats.star]);
        float damageAP = ScaleValueWithAP(caster, this.damageAP[(int)caster.stats.star]);
        float damage = damageAD + damageAP;
        effects.Add(GetPhysicalDamage(damage));

        listEffects.Add(effects);
        return listEffects;
    }
}