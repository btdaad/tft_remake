using UnityEngine;

public class Distance
{
    private int[][] _distances;
    public Distance(int rowsNb, int colsNb)
    {
        _distances = JaggedArrayUtil.InitJaggedArray<int>(rowsNb, colsNb, () => -1);
    }

    /*
     *      / \ / \                  / \ / \
     *     |x+1|x+1|                |y-1| y |
     *    / \ / \ / \              / \ / \ / \
     *   | x | x | x |    x%2==0  |y-1| y |y+1|
     *    \ / \ / \ /              \ / \ / \ / \
     *     |x-1|x-1|      x%2!=0    |y-1| y |y+1|
     *      \ / \ /                  \ / \ / \ /
     *                                | y |y+1|
     *                                 \ / \ /          
     */
    public void ComputeDistances(int x, int y)
    {
        int distance = 0;
        _distances[x][y] = distance;

        // side cells
        if (y - 1 >= 0)
            _distances[x][y - 1] = 1;
        if (y + 1 < _distances[0].Length)
            _distances[x][y + 1] = 1;

        // top/bot cells, same col
        if (x - 1 >= 0)
            _distances[x - 1][y] = 1;
        if (x + 1 < _distances.Length)
            _distances[x + 1][y] = 1;

        if (x % 2 == 0) // see diagram
        {
            if (y - 1 >= 0)
            {
                if (x - 1 >= 0)
                    _distances[x - 1][y - 1] = 1;
                if (x + 1 < _distances.Length)
                    _distances[x + 1][y - 1] = 1;
            }
        }
        else
        {
            if (y + 1 < _distances[0].Length)
            {
                if (x - 1 >= 0)
                    _distances[x - 1][y + 1] = 1;
                if (x + 1 < _distances.Length)
                    _distances[x + 1][y + 1] = 1;
            }
        }

        // side cells
        if (y - 2 >= 0)
            _distances[x][y - 2] = 2;
        if (y + 2 < _distances[0].Length)
            _distances[x][y + 2] = 2;

        // top/bot cells
        if (x - 2 >= 0)
        {
            for (int yy = y - 1; yy <= y + 1; yy++)
            {
                if (yy >= 0 && yy < _distances[0].Length)
                    _distances[x - 2][yy] = 2;
            }
        }
        if (x + 2 < _distances.Length)
        {
            for (int yy = y - 1; yy <= y + 1; yy++)
            {
                if (yy >= 0 && yy < _distances[0].Length)
                    _distances[x + 2][yy] = 2;
            }
        }

        // digonal cells
        if (x % 2 == 0)
        {
            if (y - 2 >= 0)
            {
                if (x - 1 >= 0)
                    _distances[x - 1][y - 2] = 2;
                if (x + 1 < _distances.Length)
                    _distances[x + 1][y - 2] = 2;
            }
            if (y + 1 < _distances[0].Length)
            {
                if (x - 1 >= 0)
                    _distances[x - 1][y + 1] = 2;
                if (x + 1 < _distances.Length)
                    _distances[x + 1][y + 1] = 2;
            }
        }
        else
        {
            if (y - 1 >= 0)
            {
                if (x - 1 >= 0)
                    _distances[x - 1][y - 1] = 2;
                if (x + 1 < _distances.Length)
                    _distances[x + 1][y - 1] = 2;
            }
            if (y + 2 < _distances[0].Length)
            {
                if (x - 1 >= 0)
                    _distances[x - 1][y + 2] = 2;
                if (x + 1 < _distances.Length)
                    _distances[x + 1][y + 2] = 2;
            }

        }
    }

    public void Dump()
    {
        JaggedArrayUtil.Dump(_distances);
    }
}
