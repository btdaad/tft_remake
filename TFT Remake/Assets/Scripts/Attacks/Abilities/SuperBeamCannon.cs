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
    private float secondaryDamageRatio = 0.22;
    
    public override List<Effect> GetEffect(Unit caster)
    {
        List<Effect> effects = new List<Effect>();

        float physicalDamageAD = ScaleValueWithAD(caster, damageAD[(int)caster.stats.star]);
        float physicalDamageAP = ScaleValueWithAP(caster, damageAP[(int)caster.stats.star]);
        float physicalDamage = physicalDamageAD + physicalDamageAP;
        effects.Add(GetPhysicalDamage(physicalDamage));

        return effects;
    }
}