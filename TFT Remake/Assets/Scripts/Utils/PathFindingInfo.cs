using UnityEngine;
using System;
using System.Collections.Generic;

public class PathFindingInfo
{
    public struct HexCellInfo
    {
        public int dist;
        public Coords coords;
        public Coords fromCell;

        public HexCellInfo(int dist, Coords coords, Coords fromCell)
        {
            this.dist = dist;
            this.coords = coords;
            this.fromCell = fromCell;
        }
    }

    private HexCellInfo[][] _hexCellInfos;
    public PathFindingInfo(int rowsNb, int colsNb)
    {
        _hexCellInfos = JaggedArrayUtil.InitJaggedArray<HexCellInfo>(rowsNb, colsNb, () => new HexCellInfo(-1, new Coords(-1, -1), new Coords(-1, -1)));
    }

    public int[][] GetDistances()
    {
        int[][] dist = new int[_hexCellInfos.Length][];
        for (int x = 0; x < dist.Length; x++)
        {
            dist[x] = new int[_hexCellInfos[0].Length];
            for (int y = 0; y < dist[x].Length; y++)
                dist[x][y] = _hexCellInfos[x][y].dist;
        }
        return dist;
    }

    public List<Coords> GetCellsTo(int distance)
    {
        List<Coords> coords = new List<Coords>();
        for (int x = 0; x < _hexCellInfos.Length; x++)
        {
            for (int y = 0; y < _hexCellInfos[x].Length; y++)
            {
                if (_hexCellInfos[x][y].dist <= distance)
                    coords.Add(_hexCellInfos[x][y].coords);
            }
        }
        return coords;
    }

    public HexCellInfo[][] GetHexCellInfos()
    {
        return _hexCellInfos;
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
        if (y + 1 < _hexCellInfos[0].Length)
            cells.Add(new Coords(x, y + 1));

        // top/bot cells, same col
        if (x - 1 >= 0)
            cells.Add(new Coords(x - 1, y));
        if (x + 1 < _hexCellInfos.Length)
            cells.Add(new Coords(x + 1, y));

        if (x % 2 == 0) // see diagram
        {
            if (y - 1 >= 0)
            {
                if (x - 1 >= 0)
                    cells.Add(new Coords(x - 1, y - 1));
                if (x + 1 < _hexCellInfos.Length)
                    cells.Add(new Coords(x + 1, y - 1));
            }
        }
        else
        {
            if (y + 1 < _hexCellInfos[0].Length)
            {
                if (x - 1 >= 0)
                    cells.Add(new Coords(x - 1, y + 1));
                if (x + 1 < _hexCellInfos.Length)
                    cells.Add(new Coords(x + 1, y + 1));
            }
        }

        return cells;
    }

    private bool SaveDistance(int dist, Coords curCoords, List<Coords> cells)
    {
        bool wasDistancesUpdated = false;
        foreach (Coords cellCoords in cells)
        {
            int x = cellCoords.x;
            int y = cellCoords.y;
            if (_hexCellInfos[x][y].dist == -1)
            {
                _hexCellInfos[x][y].dist = dist;
                _hexCellInfos[x][y].coords = cellCoords;
                _hexCellInfos[x][y].fromCell = curCoords;
                wasDistancesUpdated = true;
            }
            else if (_hexCellInfos[x][y].dist > dist)
            {
                _hexCellInfos[x][y].dist = dist;
                _hexCellInfos[x][y].coords = cellCoords;
                _hexCellInfos[x][y].fromCell = curCoords;
                wasDistancesUpdated = true;
            }
        }
        return wasDistancesUpdated;
    }

    private void ComputeDistancesRec(Coords curCoords, int dist)
    {
        List<Coords> cells = FindSurroundingCells(curCoords);
        bool wasDistancesUpdated = SaveDistance(dist, curCoords, cells);
        if (wasDistancesUpdated)
        {
            foreach (Coords cellCoords in cells)
                ComputeDistancesRec(cellCoords, dist + 1);
        }
    }

    public void ComputeDistances(int x, int y)
    {
        Coords curCoords = new Coords(x, y);
        _hexCellInfos[x][y].dist = 0;
        _hexCellInfos[x][y].coords = curCoords;
        _hexCellInfos[x][y].fromCell = curCoords;
        ComputeDistancesRec(curCoords, 1);
    }

    public void Dump()
    {
        JaggedArrayUtil.Dump(_hexCellInfos);
    }
}
