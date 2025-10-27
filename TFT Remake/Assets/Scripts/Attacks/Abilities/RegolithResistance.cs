using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Abilities/Malphite's")]

/* Active: Deal 189/283/426 (ArmorAP) physical damage to enemies in a zone*.

   Damage: 189/283/426 = 40/60/90 AP + 270/405/610% Armor
   
   
   *changed from cone to zone for easier implementation */
public class RegolithResistance : AbilityBase
{
    private int[] damageAP = { 40, 60, 90 };
    private int[] damageArmor = { 270, 405, 610 };
    
    public override List<Effect> GetEffect(Unit caster)
    {
        List<Effect> effects = new List<Effect>();

        float physicalDamageAP = ScaleValueWithAP(caster, damageAP[(int)caster.stats.star]);
        float physicalDamageArmor = ScaleValueWithArmor(caster, damageArmor[(int)caster.stats.star]);
        float physicalDamage = physicalDamageAP + physicalDamageArmor;
        effects.Add(GetPhysicalDamage(physicalDamage));

        // TODO : implement passive effect "Passive: Gain 12 Armor for each item equipped."

        return effects;
    }
}