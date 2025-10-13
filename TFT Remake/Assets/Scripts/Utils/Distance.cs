using UnityEngine;
using System;
using System.Collections.Generic;

public class Distance
{
    private struct HexCellInfo
    {
        public int dist;
        public Coords fromCell;

        public HexCellInfo(int dist, Coords fromCell)
        {
            this.dist = dist;
            this.fromCell = fromCell;
        }
    }
    private HexCellInfo[][] _distances;
    public Distance(int rowsNb, int colsNb)
    {
        _distances = JaggedArrayUtil.InitJaggedArray<HexCellInfo>(rowsNb, colsNb, () => new HexCellInfo(-1, new Coords(-1, -1)));
    }

    public int[][] GetDistances()
    {
        int[][] dist = new int[_distances.Length][];
        for (int x = 0; x < dist.Length; x++)
        {
            dist[x] = new int[_distances[0].Length];
            for (int y = 0; y < dist[x].Length; y++)
                dist[x][y] = _distances[x][y].dist;
        }
        return dist;
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
    private List<Coords> FindSurroundingCells(Coords coords)
    {
        int x = coords.x;
        int y = coords.y;
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

    private bool SaveDistance(int dist, Coords coords, List<Coords> cells)
    {
        bool wasDistancesUpdated = false;
        foreach (Coords coord in cells)
        {
            int x = coord.x;
            int y = coord.y;
            if (_distances[x][y].dist == -1)
            {
                _distances[x][y].dist = dist;
                _distances[x][y].fromCell = coords;
                wasDistancesUpdated = true;
            }
            else if (_distances[x][y].dist > dist)
            {
                _distances[x][y].dist = dist;
                _distances[x][y].fromCell = coords;
                wasDistancesUpdated = true;
            }
        }
        return wasDistancesUpdated;
    }

    private void ComputeDistancesRec(Coords coords, int dist)
    {
        List<Coords> cells = FindSurroundingCells(coords);
        bool wasDistancesUpdated = SaveDistance(dist, coords, cells);
        if (wasDistancesUpdated)
        {
            foreach (Coords cellCoords in cells)
                ComputeDistancesRec(cellCoords, dist + 1);
        }
    }

    public void ComputeDistances(int x, int y)
    {
        Coords coords = new Coords(x, y);
        _distances[x][y].dist = 0;
        _distances[x][y].fromCell = coords;
        ComputeDistancesRec(coords, 1);
    }

    public void Dump()
    {
        JaggedArrayUtil.Dump(_distances);
    }
}
