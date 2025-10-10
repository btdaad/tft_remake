using UnityEngine;

public class PvPManager : MonoBehaviour
{
    private bool _fight = false;
    private GameManager _gameManager;
    private Distance[][] _distances;
    void Init()
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

    private Coord FindClosestUnit(List<Coord> unitsCoord, int index)
    {
        Coord unitCoord = unitsCoord[index];
        int[][] distances = _distances[unitCoord.x][unitCoord.y].GetDistances();

        Coord minDistCoord = unitCoord;
        int minDist = int.MaxValue;
        for (int i = 0; i < unitsCoord.Count; i++)
        {
            if (i != index)
            {
                int dist = distances[unitsCoord[i].x][unitsCoord[i].y];
                if (dist < minDist)
                {
                    minDist = dist;
                    minDistCoord = unitsCoord[i];
                }
            }
        }

        return minDistCoord;
    }

    private List<Coord> GetUnitsCoord(Transform[][] units)
    {
        List<Coords> unitsCoords = new List<Coords>();
        for (int x = 0; x < units.Length; x++)
        {
            for (int y = 0; y < units[0].Length; y++)
            {
                if (units[x][y] != null)
                    unitsCoords.Add(new Coord(x, y));
            }
        }

        return unitsCoords;
    }

    private void MoveUnitTo(Transform unitTransform, int x, int y)
    {
        
    }

    public void MoveUnits()
    {
        Transform[][] units = _gameManager.GetBoardManager().GetBattlefield();
        List<Coord> unitsCoord = GetUnitsCoord(units);

        for (int i = 0; i < unitsCoords.Count; i++)
        {
            Coord closestCoord = FindClosestUnit(unitsCoord, i);
            Coord unitCoord = unitsCoord[i];
            MoveUnitTo(units[unitCoord.x][unitCoord.y], closestCoord.x, closestCoord.y);
        }
    }
}
