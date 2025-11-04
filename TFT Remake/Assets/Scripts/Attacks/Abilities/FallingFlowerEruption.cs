using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Abilities/Neeko's")]

/* Gain 40% Durability and heal 300/350/450 (AP) over 2 seconds,
   then deal 120/180/290 (AP) magic damage to enemies in a 2-hex radius.
   
   Heal: 300/350/450 (AP)
   Damage: 120/180/290 (AP)*/
public class FallingFlowerEruption : AbilityBase
{
    private float durability = 0.40f;
    private int[] healAP = { 300, 350, 450 };
    float healTime = 2.0f;
    private int[] damageAP = { 120, 180, 290 };
    
    public override List<List<Effect>> GetEffects(Unit caster)
    {
        caster.GainDurability(durability, healTime);
        caster.UpdateHealth(ScaleValueWithAD(caster, healAP[(int)caster.stats.star]), healTime);

        List<List<Effect>> listEffects = new List<List<Effect>>();

        List<Effect> effects = new List<Effect>();
        float magicDamageAP = ScaleValueWithAP(caster, damageAP[(int)caster.stats.star]);
        effects.Add(GetMagicDamage(magicDamageAP));

        listEffects.Add(effects);
        return listEffects;
    }
}