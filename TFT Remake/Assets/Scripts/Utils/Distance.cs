using UnityEngine;
using System;
using System.Collections.Generic;

public class Distance
{
    private int[][] _distances;
    public Distance(int rowsNb, int colsNb)
    {
        _distances = JaggedArrayUtil.InitJaggedArray<int>(rowsNb, colsNb, () => -1);
    }

    private struct Coords
    {
        public int x;
        public int y;
        public Coords(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
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
    private List<Coords> FindSurroundingCells(int x, int y)
    {
        List<Coords> cells = new List<Coords>();

        // side cells
        if (y - 1 >= 0)
            cells.Add(new Coords(x, y - 1));
        if (y + 1 < _distances[0].Length)
            cells.Add(new Coords(x, y + 1));

        // top/bot cells, same col
        if (x - 1 >= 0)
            cells.Add(new Coords(x - 1, y));
        if (x + 1 < _distances.Length)
            cells.Add(new Coords(x + 1, y));

        if (x % 2 == 0) // see diagram
        {
            if (y - 1 >= 0)
            {
                if (x - 1 >= 0)
                    cells.Add(new Coords(x - 1, y - 1));
                if (x + 1 < _distances.Length)
                    cells.Add(new Coords(x + 1, y - 1));
            }
        }
        else
        {
            if (y + 1 < _distances[0].Length)
            {
                if (x - 1 >= 0)
                    cells.Add(new Coords(x - 1, y + 1));
                if (x + 1 < _distances.Length)
                    cells.Add(new Coords(x + 1, y + 1));
            }
        }

        return cells;
    }

    private bool SaveDistance(int dist, List<Coords> cells)
    {
        bool wasDistancesUpdated = false;
        foreach (Coords coord in cells)
        {
            int x = coord.x;
            int y = coord.y;
            if (_distances[x][y] == -1)
            {
                _distances[x][y] = dist;
                wasDistancesUpdated = true;
            }
            else if (_distances[x][y] > dist)
            {
                _distances[x][y] = dist;
                wasDistancesUpdated = true;
            }
        }
        return wasDistancesUpdated;
    }

    private void ComputeDistancesRec(int x, int y, int dist)
    {
        List<Coords> cells = FindSurroundingCells(x, y);
        bool wasDistancesUpdated = SaveDistance(dist, cells);
        if (wasDistancesUpdated)
        {
            foreach (Coords coord in cells)
                ComputeDistancesRec(coord.x, coord.y, dist + 1);
        }
    }

    public void ComputeDistances(int x, int y)
    {
        _distances[x][y] = 0;
        ComputeDistancesRec(x, y, 1);
    }

    public void Dump()
    {
        JaggedArrayUtil.Dump(_distances);
    }
}
