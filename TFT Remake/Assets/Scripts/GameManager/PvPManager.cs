using UnityEngine;
using System.Collections.Generic;

public class PvPManager : MonoBehaviour
{
    private bool _hasFightStarted = false;
    private GameManager _gameManager;
    private PathFindingInfo[][] _pathFindingInfo;
    public void Init()
    {
        _gameManager = GameManager.Instance;
        _pathFindingInfo = _gameManager.GetBoardManager().GetDistances();
    }

    void Update()
    {
        if (_hasFightStarted)
        {
            InvokeRepeating(nameof(MoveUnits), 0.0f, 5.0f);
            _hasFightStarted = false;
        }
    }
    public void Fight()
    {
        _hasFightStarted = true;
    }

    private (Coords, int) FindClosestUnit(List<Coords> coordsList, int index)
    {
        Coords coords = coordsList[index];
        int[][] distances = _pathFindingInfo[coords.x][coords.y].GetDistances();

        Coords minDistCoords = coords;
        int minDist = int.MaxValue;
        for (int i = 0; i < coordsList.Count; i++)
        {
            if (i != index)
            {
                int dist = distances[coordsList[i].x][coordsList[i].y];
                if (dist < minDist)
                {
                    minDist = dist;
                    minDistCoords = coordsList[i];
                }
            }
        }

        return (minDistCoords, minDist);
    }

    private List<Coords> GetUnitsCoords(Transform[][] units)
    {
        List<Coords> coordsList = new List<Coords>();
        for (int x = 0; x < units.Length; x++)
        {
            for (int y = 0; y < units[0].Length; y++)
            {
                if (units[x][y] != null)
                    coordsList.Add(new Coords(x, y));
            }
        }

        return coordsList;
    }

    private Coords FindNextCell(Coords curCoords, Coords targetCoords)
    {
        PathFindingInfo.HexCellInfo[][] hexCellInfos = _pathFindingInfo[curCoords.x][curCoords.y].GetHexCellInfos();
        PathFindingInfo.HexCellInfo nextCell = hexCellInfos[targetCoords.x][targetCoords.y];
        while (nextCell.fromCell != curCoords)
            nextCell = hexCellInfos[nextCell.fromCell.x][nextCell.fromCell.y];

        return nextCell.coords;
    }

    private void MoveUnitTo(Coords curCoords, Coords targetCoords)
    {
        _gameManager.GetBoardManager().MoveUnitTo(curCoords, targetCoords);
    }

    public void MoveUnits()
    {
        Transform[][] units = _gameManager.GetBoardManager().GetBattlefield();
        // differentiate friend and enemi teams !!!
        List<Coords> coordsList = GetUnitsCoords(units);

        for (int i = 0; i < coordsList.Count; i++)
        {
            Coords curCoords = coordsList[i];
            (Coords closestCoords, int dist) = FindClosestUnit(coordsList, i);

            Transform unitTransform = units[curCoords.x][curCoords.y];
            UnitStats stats = unitTransform.GetComponent<Unit>().stats;
            int range = stats.range;

            if (range < dist) // if cannot attack from this distance, unit have to move closer
            {
                Coords nextCellCoords = FindNextCell(curCoords, closestCoords);
                MoveUnitTo(curCoords, nextCellCoords);
            }
        }
    }
}
