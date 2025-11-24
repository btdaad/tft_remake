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
    
    public override List<List<Effect>> GetEffects(Unit caster)
    {
        List<List<Effect>> listEffects = new List<List<Effect>>();

        float missingHealth = - (caster.GetHealth() - caster.GetMaxHealth());
        float missingHealthRatio = missingHealth / caster.GetMaxHealth();
        float missingHealthPercent = Mathf.Lerp(0, 1, missingHealthRatio);
        // Debug.Log($"[{Time.time}] ({caster.gameObject.name}): missingHealthPercent = {missingHealthPercent} for {caster.GetHealth()}/{caster.GetMaxHealth()}");

        List<Effect> effects = new List<Effect>();

        float physicalDamageAD = ScaleValueWithAD(caster, damageAD[(int)caster.GetStar()]);
        float physicalDamageAP = ScaleValueWithAP(caster, damageAP[(int)caster.GetStar()]);
        float physicalDamage = physicalDamageAD + physicalDamageAP;
        // Debug.Log($"[{Time.time}] ({caster.gameObject.name}): physicalDamage = {physicalDamageAD} + {physicalDamageAP} = {physicalDamage}");
        physicalDamage += missingHealthPercent * physicalDamage;
        // Debug.Log($"[{Time.time}] ({caster.gameObject.name}): physicalDamage += {missingHealthPercent}% = {physicalDamage}");
        effects.Add(GetPhysicalDamage(physicalDamage));

        listEffects.Add(effects);

        float heal = ScaleValueWithAP(caster, healAP[(int)caster.GetStar()]) + healHealth * caster.GetMaxHealth();
        // Debug.Log($"[{Time.time}] ({caster.gameObject.name}): heal = {heal} + {missingHealthPercent}%");
        heal += missingHealthPercent * heal;

        // Debug.Log($"[{Time.time}] ({caster.gameObject.name}): caster health = {caster.GetHealth()}");
        caster.UpdateHealth(heal, 0f);
        // Debug.Log($"[{Time.time}][HEALED] ({caster.gameObject.name}): caster health = {caster.GetHealth()}");

        return listEffects;
    }
}