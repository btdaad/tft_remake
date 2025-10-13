using UnityEngine;
using System.Collections.Generic;

public class PvPManager : MonoBehaviour
{
    private bool _fight = false;
    private GameManager _gameManager;
    private Distance[][] _distances;
    public void Init()
    {
        _gameManager = GameManager.Instance;
        _distances = _gameManager.GetBoardManager().GetDistances();
    }

    void Update()
    {
        if (_fight)
            MoveUnits();
    }
    public void Fight()
    {
        _fight = true;
    }

    private (Coords, int) FindClosestUnit(List<Coords> coordsList, int index)
    {
        Coords coords = coordsList[index];
        int[][] distances = _distances[coords.x][coords.y].GetDistances();

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

    private void MoveUnitTo(Transform unitTransform, int x, int y)
    {
        
    }

    public void MoveUnits()
    {
        Transform[][] units = _gameManager.GetBoardManager().GetBattlefield();
        List<Coords> coordsList = GetUnitsCoords(units);

        for (int i = 0; i < coordsList.Count; i++)
        {
            Coords coords = coordsList[i];
            (Coords closestCoords, int dist) = FindClosestUnit(coordsList, i);

            Transform unitTransform = units[coords.x][coords.y];
            UnitStats stats = unitTransform.GetComponent<UnitStats>();
            int range = stats.range;
            
            if (range < dist)
                MoveUnitTo(units[coords.x][coords.y], closestCoords.x, closestCoords.y);
        }
    }
}
