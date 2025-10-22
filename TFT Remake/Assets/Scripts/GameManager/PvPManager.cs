using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            _gameManager.GetBoardManager().SavePositions();
            InvokeRepeating(nameof(MoveUnits), 0.0f, 0.3f);
            _hasFightStarted = false;
        }
    }

    private void HandleUnitDeath(object sender, EventArgs e)
    {
        OnDeathEventArgs onDeathEventArgs = (OnDeathEventArgs)e;
        _gameManager.GetBoardManager().RemoveUnit(onDeathEventArgs.unit);
    }

    public void Fight()
    {
        _hasFightStarted = true;

        Transform[][] units = _gameManager.GetBoardManager().GetBattlefield();
        for (int x = 0; x < units.Length; x++)
        {
            for (int y = 0; y < units[0].Length; y++)
            {
                if (units[x][y] != null)
                    units[x][y].GetComponent<Unit>().OnDeath += HandleUnitDeath;
            }
        }
    }

    private void DestroyLastAttackSpheres()
    {
        GameObject[] attackSpheres = GameObject.FindGameObjectsWithTag("Attack");
        foreach (GameObject attackSphere in attackSpheres)
            Destroy(attackSphere);
    }

    private async Task EndFight(bool hasPlayerWon, bool hasOpponentWon, int playerTeamSize, int opponentTeamSize)
    {
        CancelInvoke(nameof(MoveUnits));

        int damage = playerTeamSize;
        if (hasOpponentWon)
            damage = opponentTeamSize;
        _gameManager.EndFight(hasPlayerWon, damage);

        DestroyLastAttackSpheres();

        await Task.Delay(1000);

        _gameManager.GetBoardManager().RestorePositions();
        _gameManager.ManageGold();

        // TODO : take stages into account to remove hp
    }

    private (Coords, int) FindClosestUnit(List<Coords> coordsList, Coords curCoords)
    {
        int[][] distances = _pathFindingInfo[curCoords.x][curCoords.y].GetDistances();

        Coords minDistCoords = curCoords;
        int minDist = int.MaxValue;
        for (int i = 0; i < coordsList.Count; i++)
        {
            int dist = distances[coordsList[i].x][coordsList[i].y];
            if (dist < minDist)
            {
                minDist = dist;
                minDistCoords = coordsList[i];
            }
        }

        return (minDistCoords, minDist);
    }

    // Extracts unit coordinates from the board manager grid
    private (List<Coords>, List<Coords>) GetUnitsCoords(Transform[][] units)
    {
        List<Coords> playerTeamCoords = new List<Coords>();
        List<Coords> opponentTeamCoords = new List<Coords>();
        for (int x = 0; x < units.Length; x++)
        {
            for (int y = 0; y < units[0].Length; y++)
            {
                if (units[x][y] != null)
                {
                    if (units[x][y].GetComponent<Unit>().IsFromPlayerTeam())
                        playerTeamCoords.Add(new Coords(x, y));
                    else
                        opponentTeamCoords.Add(new Coords(x, y));
                }
            }
        }

        return (playerTeamCoords, opponentTeamCoords);
    }

    // Revert path to closest cell in order to find the cell to move to, in order to get closer
    private Coords FindNextCell(Coords curCoords, Coords targetCoords)
    {
        PathFindingInfo.HexCellInfo[][] hexCellInfos = _pathFindingInfo[curCoords.x][curCoords.y].GetHexCellInfos();
        PathFindingInfo.HexCellInfo nextCell = hexCellInfos[targetCoords.x][targetCoords.y];
        while (nextCell.fromCell != curCoords)
            nextCell = hexCellInfos[nextCell.fromCell.x][nextCell.fromCell.y];

        return nextCell.coords;
    }

    // Move the unit on the grid and on the board
    private bool MoveUnitTo(Coords curCoords, Coords targetCoords)
    {
        return _gameManager.GetBoardManager().MoveUnitTo(curCoords, targetCoords);
    }

    // Handles logical checks before actually moving the unit
    // Has the unit already moved ? Is it close enough for its attack range ?
    private bool TryMoveUnit(Transform[][] units, List<Coords> thisTeamCoords, List<Coords> opponentTeamCoords, int index)
    {
        Coords curCoords = thisTeamCoords[index];

        Transform unitTransform = units[curCoords.x][curCoords.y];
        if (unitTransform == null) // if unit died in the meantime
            return false;

        Unit unit = unitTransform.GetComponent<Unit>();
        if (unit.HasMoved()) // as we update unit positions and recalculate the list of coordinates every time, this property prevent from moving the same entity everytime
            return false;
        unit.SetHasMoved(true);

        (Coords closestCoords, int dist) = FindClosestUnit(opponentTeamCoords, curCoords);
        int range = unit.GetRange();
        if (range < dist)
        {
            Coords nextCellCoords = FindNextCell(curCoords, closestCoords);
            return MoveUnitTo(curCoords, nextCellCoords);
        }
        else
            unit.Attack(units[closestCoords.x][closestCoords.y]);

        return false;
    }

    // Resets the hasMoved parameter that prevents a unit from moving multiple times in a row
    private void ResetHasMovedParam()
    {
        Transform[][] units = _gameManager.GetBoardManager().GetBattlefield();

        for (int x = 0; x < units.Length; x++)
        {
            for (int y = 0; y < units[0].Length; y++)
            {
                if (units[x][y] != null)
                    units[x][y].GetComponent<Unit>().SetHasMoved(false);
            }
        }
    }

    private bool CheckForAWinner(List<Coords> playerCoordsList, List<Coords> opponentCoordsList)
    {
        bool hasPlayerWon = opponentCoordsList.Count == 0;
        bool hasOpponentWon = playerCoordsList.Count == 0;

        if (hasPlayerWon || hasOpponentWon)
        {
            EndFight(hasPlayerWon, hasOpponentWon, playerCoordsList.Count, opponentCoordsList.Count);
            return true;
        }
        return false;
    }

    // Go through all units and make them move, towards their closes enemy, once each
    public void MoveUnits()
    {
        Transform[][] units = _gameManager.GetBoardManager().GetBattlefield();

        (List<Coords> playerCoordsList, List<Coords> opponentCoordsList) = GetUnitsCoords(units); // differentiate player and enemy teams

        int playerIndex = 0;
        int opponentIndex = 0;

        bool allPlayerUnitsHasMoved = false;
        bool allOpponentUnitsHasMoved = false;

        while (!(allPlayerUnitsHasMoved && allOpponentUnitsHasMoved) && !CheckForAWinner(playerCoordsList, opponentCoordsList))
        {
            bool hasMoved = false;
            if (playerIndex < playerCoordsList.Count)
            {
                hasMoved |= TryMoveUnit(units, playerCoordsList, opponentCoordsList, playerIndex);
                playerIndex++;
            }
            else
                allPlayerUnitsHasMoved = true;

            if (hasMoved) // a unit has moved, the coordinates have changed
            {
                units = _gameManager.GetBoardManager().GetBattlefield();
                (playerCoordsList, opponentCoordsList) = GetUnitsCoords(units);
                playerIndex = 0; // the list might be in a different order, unit already seen will be skipped because of the hasMoved parameter
                opponentIndex = 0;
            }

            if (opponentIndex < opponentCoordsList.Count)
            {
                hasMoved |= TryMoveUnit(units, opponentCoordsList, playerCoordsList, opponentIndex);
                opponentIndex++;
            }
            else
                allOpponentUnitsHasMoved = true;

            if (hasMoved) // a unit has moved, the coordinates have changed
            {
                units = _gameManager.GetBoardManager().GetBattlefield();
                (playerCoordsList, opponentCoordsList) = GetUnitsCoords(units);
                playerIndex = 0; // the list might be in a different order, unit already seen will be skipped because of the hasMoved parameter
                opponentIndex = 0;
            }
        }

        ResetHasMovedParam();
    }
}
