using UnityEngine;
using System;
using System.Text;

public static class StatUtil
{
    public static string ToTexture(BaseItemSO.Stat stat)
    {
        string str = stat.ToString();
        if (str.Length > 2)
        {
            string str2 = str[0].ToString();
            str = str.ToLower();
            bool afterUnderscore = false;
            for (int i = 1; i < str.Length; i++)
            {
                if (!afterUnderscore)
                {
                    if (str[i] == '_')
                        afterUnderscore = true;
                    else
                        str2 += str[i];
                }
                else
                {
                    str2 += Char.ToUpper(str[i]);
                    afterUnderscore = false;
                }
            }
            str = str2;
        }
        return str;
    }
}
