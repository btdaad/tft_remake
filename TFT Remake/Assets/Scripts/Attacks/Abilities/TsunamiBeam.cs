using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Abilities/Ryze's")]

/* Deal 810/1215/7000 (AP) magic damage to the target over 3 seconds.
   Enemies in a 1-hex radius around the target take 110/165/1200 (AP) over the same duration. */
public class TsunamiBeam : AbilityBase
{
    private int[] damageAP = { 810, 1215, 7000 };
    private int[] hexDamageAP = { 110, 165, 1200 };
    private float duration = 3f;
    
    public override List<List<Effect>> GetEffects(Unit caster)
    {
        List<List<Effect>> listEffects = new List<List<Effect>>();

        List<Effect> effects = new List<Effect>();

        float damage = ScaleValueWithAP(caster, damageAP[(int)caster.GetStar()]);
        float hexDamage = ScaleValueWithAP(caster, hexDamageAP[(int)caster.GetStar()]);
        effects.Add(GetMagicDamage(damage - hexDamage, duration));

        listEffects.Add(effects);

        List<Effect> hexEffects = new List<Effect>();

        hexEffects.Add(GetMagicDamage(hexDamage, duration));

        listEffects.Add(hexEffects);

        return listEffects;
    }
}