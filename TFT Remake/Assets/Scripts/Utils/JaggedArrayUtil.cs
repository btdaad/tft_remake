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
}
