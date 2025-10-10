using UnityEngine;
using System;

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
        _distances[x][y] = 0;

        int distance = 1;
        for (int yy = y + 1; yy < _distances[0].Length; yy++) // fill, same row, columns on the right
            _distances[x][yy] = distance++; // distance increments for each cell
        distance = 1;
        for (int yy = y - 1; yy >= 0; yy--) // fill, same rows, columns on the left; 
            _distances[x][yy] = distance++;

        // for the rows above
        for (int xx = x + 1; xx < _distances.Length; xx++)
        {
            distance = xx - x;
            int yy = y;
            // fill, in row xx, the cells at same distance
            if (x % 2 == 0)
            {
                for (; yy < _distances[0].Length && (yy - y) <= Math.Ceiling((float)distance / 2.0f); yy++) // the ceiling thing is based on observations, see diagram
                    _distances[xx][yy] = distance;
            }
            else
            {
                for (; yy < _distances[0].Length && (yy - y) <= Math.Floor((float)distance / 2.0f); yy++)
                    _distances[xx][yy] = distance;
            }

            // fill, in row xx, the columns on the right
            for (; yy < _distances[0].Length; yy++)
                _distances[xx][yy] = ++distance;

            distance = xx - x;
            yy = y - 1;
            if (x % 2 == 0)
            {
                for (; yy >= 0 && y - yy <= Math.Floor((float) distance / 2.0f); yy--)
                    _distances[xx][yy] = distance;
            }
            else
            {
                for (; yy >= 0 && y - yy <= Math.Ceiling((float) distance / 2.0f); yy--)
                    _distances[xx][yy] = distance;
            }
            
            // fill, in row xx, the columns on the left
            for (; yy >= 0; yy--)
                _distances[xx][yy] = ++distance;
        }
    }

    public void Dump()
    {
        JaggedArrayUtil.Dump(_distances);
    }
}
