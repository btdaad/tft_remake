using UnityEngine;

public class Distance
{
    private int[][] _distances;
    public Distance(int rowsNb, int colsNb)
    {
        _distances = JaggedArrayUtil.InitJaggedArray<int>(rowsNb, colsNb, () => -1);
    }

    public void ComputeDistances(int x, int y)
    {
        int distance = 0;
        _distances[x][y] = distance;
    }
}
