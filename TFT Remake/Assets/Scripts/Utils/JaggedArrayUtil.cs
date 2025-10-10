using UnityEngine;
using System;

public static class JaggedArrayUtil
{
    public static T[][] InitJaggedArray<T>(int rowsNb, int colsNb, Func<T> defaultValue)
    {
        T[][] jaggedArray = new T[rowsNb][];
        for (int x = 0; x < rowsNb; x++)
        {
            jaggedArray[x] = new T[colsNb];
            for (int y = 0; y < colsNb; y++)
                jaggedArray[x][y] = defaultValue();
        }
        return jaggedArray;
    } 
    public static void Dump<T>(T[][] board)
    {
        string str = "[";
        foreach (var row in board)
        {
            str += "[";
            int i = 0;
            for (; i < row.Length - 1; i++)
                str += row[i] + ", ";
            str += row[i];
            str += "]\n";
        }
        if (str[str.Length - 1] == '\n')
            str = str.Remove(str.Length - 1);
        Debug.Log(str + "]");
    }
}
