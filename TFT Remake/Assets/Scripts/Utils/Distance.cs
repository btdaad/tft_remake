using UnityEngine;

public class Distance 
{
    public Distance(int rowsNb, int colsNb)
    {
        distances = JaggedArrayUtil.InitJaggedArray<int>(rowsNb, colsNb, () => -1);
    }
    public int[][] distances;
}
