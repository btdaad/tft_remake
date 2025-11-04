using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Abilities/Senna's")]

/* Over 1 second, deal 400/600/970 (AD/AP) physical damage in a line through the current target.
   Enemies after the first take 88/132/213 (AD/AP) instead.

   When no targets are left, regain mana based on the time remaining
  
   Damage: (AP) 40/60/95
   Damage: 400/600/970 = 360/540/875 (AD) + 40/60/95 (AP)
   Secondary Damage: 88/132/213 = 22% of 400/600/970 */
public class SuperBeamCannon : AbilityBase
{
    private int[] damageAP = { 40, 60, 95 };
    private int[] damageAD = { 360, 540, 875 };
    private float secondaryDamageRatio = 0.22f;
    
    public override List<List<Effect>> GetEffects(Unit caster)
    {
        List<List<Effect>> listEffects = new List<List<Effect>>();

        // Dammage to first target
        List<Effect> firstTargetEffects = new List<Effect>();
        float damageAD = ScaleValueWithAD(caster, this.damageAD[(int)caster.stats.star] * (1 - secondaryDamageRatio));
        float damageAP = ScaleValueWithAP(caster, this.damageAP[(int)caster.stats.star] * (1 - secondaryDamageRatio));
        float damage = damageAD + damageAP;
        firstTargetEffects.Add(GetPhysicalDamage(damage));
        listEffects.Add(firstTargetEffects);

        // Dammage to other targets AMD first target
        List<Effect> otherTargetsEffects = new List<Effect>();
        damageAD = ScaleValueWithAD(caster, this.damageAD[(int)caster.stats.star] * secondaryDamageRatio);
        damageAP = ScaleValueWithAP(caster, this.damageAP[(int)caster.stats.star] * secondaryDamageRatio);
        damage = damageAD + damageAP;
        otherTargetsEffects.Add(GetPhysicalDamage(damage));
        listEffects.Add(otherTargetsEffects);

        return listEffects;
    }
}