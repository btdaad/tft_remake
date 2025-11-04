using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Abilities/Ziggs's")]

/* Passive: Attacks are bouncing bombs that deal 70/105/165 (APAD) magic damage to the first enemy hit. // TODO
   Active: Deal 230/345/550 (AP) magic damage in a 1-hex radius centered on the target.
   
   Passive Damage: 70/105/165 = 20 (AD) + 50/75/120 (AP)
   Damage: 230/345/550 (AP) */
public class OrbitalOrdinance : AbilityBase
{
    private int[] damageAP = { 230, 345, 550 };
    
    public override List<List<Effect>> GetEffects(Unit caster)
    {
        List<List<Effect>> listEffects = new List<List<Effect>>();

        List<Effect> effects = new List<Effect>();

        float damage = ScaleValueWithAP(caster, damageAP[(int)caster.stats.star]);
        effects.Add(GetMagicDamage(damage));

        listEffects.Add(effects);
        return listEffects;
    }
}