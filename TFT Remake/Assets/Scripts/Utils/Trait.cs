using UnityEngine;
using System;
using System.Text;

public enum Trait
{
    None,
    Prism,
    Oblong,
    Sharp,
    Cinqo,
    DoubleQuatro,
    Wobbly,
    SoloBolo
};

public static class UnitTrait
{
    public static string ToTexture(Trait trait)
    {
        StringBuilder strBuilder = new StringBuilder(trait.ToString());
        strBuilder[0] = Char.ToLower(strBuilder[0]);
        return strBuilder.ToString();
    }

    public static string ToString(Trait trait)
    {
        string str = trait.ToString();
        string formatted = str[0].ToString();
        for (int i = 1; i < str.Length; i++)
        {
            if (Char.IsUpper(str[i]))
                formatted += ' ';
            formatted += str[i];
        }
        return formatted;
    }
}
