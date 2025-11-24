using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Abilities/Naafiri's")]

/* Gain 90/110/150 (AP) shield for 2 seconds, then deal 150/225/340 (AD) physical damage to the target. */
public class SpinningSoulSaw : AbilityBase
{
    private int[] shieldAP = { 90, 110, 150 };
    float shieldTime = 2.0f;
    private int[] damageAD = { 150, 225, 340 };
    
    public override List<List<Effect>> GetEffects(Unit caster)
    {
        caster.SetShield(ScaleValueWithAP(caster, shieldAP[(int)caster.GetStar()]), shieldTime);

        List<List<Effect>> listEffects = new List<List<Effect>>();

        List<Effect> effects = new List<Effect>();
        float damage = ScaleValueWithAD(caster, damageAD[(int)caster.GetStar()]);
        effects.Add(GetPhysicalDamage(damage));

        listEffects.Add(effects);
        return listEffects;
    }
}