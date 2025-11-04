using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Abilities/Braum's")]

/* Stun the target and spin them around for 1.5 seconds, dealing 575/865/9001 (AD)
   physical damage to the target and 310/465/4501 (AD) to other enemies in a 2-hex radius.
   
   Damage: 575/865/9001 (AD)
   Damage: 310/465/4501 (AD)

   Then throw the target forward, dealing 100/150/2000 (AD) in a 1-hex area around the target.
   If the target drops below 18% (AP) health, throw them off the board instead. // NOT IMPLEMENTED (TODO?)
   
   Toss Damage: 100/150/2000 (AD)
   Execute Threshold: 18% (AP) */
public class ElTigreTornado : AbilityBase
{
    private float stunTime = 1.5f;
    private int[] damageAD = { 575, 865, 9001 };
    private int[] hexDamageAD = { 310, 465, 4501 };
    private int[] tossDamageAD = { 100, 150, 2000 };
    // private float executeThreshold = 18;
    
    public override List<List<Effect>> GetEffects(Unit caster)
    {
        List<List<Effect>> listEffects = new List<List<Effect>>();

        List<Effect> effects = new List<Effect>();

        effects.Add(new Effect(stunTime, EffectType.STUN));
        
        float physicalDamageAD = ScaleValueWithAD(caster, damageAD[(int)caster.stats.star]);
        float physicalDamageADHex = ScaleValueWithAD(caster, hexDamageAD[(int)caster.stats.star]);
        effects.Add(GetPhysicalDamage(physicalDamageAD - physicalDamageADHex));

        listEffects.Add(effects); // apply damage to target

        List<Effect> effectsHex = new List<Effect>();
        effectsHex.Add(GetPhysicalDamage(physicalDamageADHex));

        listEffects.Add(effectsHex); // apply damage in 2-hex radius

        List<Effect> effectsToss = new List<Effect>();
        float physicalDamageADToss = ScaleValueWithAD(caster, tossDamageAD[(int)caster.stats.star]);
        effectsToss.Add(GetPhysicalDamage(physicalDamageADToss));

        listEffects.Add(effectsToss); // apply damage in 1-hex radius

        return listEffects;
    }
}