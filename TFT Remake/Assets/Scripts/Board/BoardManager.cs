using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;

// Vocabulary note :
// the "battlefield" : the game board part where active units are placed, where the fight happens
// the "bench" : the game board part used to store passive units 
// a "zone" : a part of the game board, usually either the battlefield or the bench

// it is expected that a method mentionning "Zone" can handle both the battlefield and the bench
// while "Board" represents the data structure (jagged array) actually used to store information (not the "physical" game board)
// it is expected that a method mentionning "Board" handle both the battlefield and the bench data structures 

public class BoardManager : MonoBehaviour
{
    private static BoardManager _instance;
    private static Transform[][] _battlefieldGrid = null;
    private static Transform[][] _benchGrid = null;
    private static Transform[][] _saveBattlefieldGrid = null;
    private static List<GameObject> _saveUnits = null;
    private static PathFindingInfo[][] _pathFindingInfo = null;
    public static BoardManager Instance(Tilemap battlefieldTilemap, Tilemap benchTilemap)
    {
        if (_battlefieldGrid == null || _benchGrid == null)
        {
            InitBoard(battlefieldTilemap, ref _battlefieldGrid, false);
            InitBoard(benchTilemap, ref _benchGrid, true);
            InitPathFindingInfo();
        }
        return _instance;
    }

    private PlayerBoardManager _playerBoardManager;
    private PlayerBoardManager _opponentBoardManager;
    public event EventHandler MoveUnit = delegate { };
    [SerializeField] GameObject arrowHelperPrefab;

    public void Init()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
            _instance = this;

        _playerBoardManager = new PlayerBoardManager("Player", this);
        _opponentBoardManager = new PlayerBoardManager("Opponent", this);
        MoveUnit = GameManager.Instance.UpdateSynergies; // add UpdateSynergies to the subscribers
    }

    public bool IsMaxTeamSize()
    {
        bool affiliation = GameManager.Instance.isPlayer;

        int teamSize = 0;
        for (int x = 0; x < _battlefieldGrid.Length; x++)
        {
            for (int y = 0; y < _battlefieldGrid[x].Length; y++)
            {
                Transform unitTransform = _battlefieldGrid[x][y];
                if (unitTransform != null && unitTransform.GetComponent<Unit>().IsFromPlayerTeam() == affiliation)
                    teamSize++;
            }
        }

        return teamSize == GameManager.Instance.GetPlayer(affiliation).GetLevel(); // max team size == player level
    }    

    public bool OnDragUnit(bool isPlayer, Transform unitTransform)
    {
        if (isPlayer)
            return _playerBoardManager.OnDragUnit(unitTransform);
        else
            return _opponentBoardManager.OnDragUnit(unitTransform);
    }
    public void OnDropUnit(bool isPlayer, Transform unitTransform)
    {
        if (isPlayer)
            _playerBoardManager.OnDropUnit(unitTransform);
        else
            _opponentBoardManager.OnDropUnit(unitTransform);
    }

    // Implemented only for battlefield units
    public List<Transform> GetUnitsAt(List<Coords> coords)
    {
        List<Transform> units = new List<Transform>();
        foreach (Coords coord in coords)
        {
            if (_battlefieldGrid[coord.x][coord.y] != null)
                units.Add(_battlefieldGrid[coord.x][coord.y]);
        }
        return units;
    }

    public Transform GetUnitAt(int xPos, int yPos, bool isBattlefield)
    {
        if (isBattlefield)
            return _battlefieldGrid[yPos][xPos];
        else
            return _benchGrid[yPos][xPos];
    }

    public void SetUnitAt(int xPos, int yPos, Transform unitTransform, bool isBattlefield)
    {
        if (isBattlefield)
            _battlefieldGrid[yPos][xPos] = unitTransform;
        else
            _benchGrid[yPos][xPos] = unitTransform;
    }

    public void CallMoveUnit(object sender, MoveUnitEventArgs moveUnitEventArgs)
    {
        MoveUnit(sender, moveUnitEventArgs);
    }

    public Transform[][] GetBattlefield()
    {
        return _battlefieldGrid;
    }

    public PathFindingInfo[][] GetDistances()
    {
        return _pathFindingInfo;
    }

    private static void InitBoard(Tilemap tilemap, ref Transform[][] board, bool isBench)
    {
        BoundsInt bounds = tilemap.cellBounds;
        Vector3Int boardSize = bounds.size + bounds.position;
        board = new Transform[isBench ? 2 : boardSize.y][];
        for (int i = 0; i < board.Length; i++)
            board[i] = new Transform[boardSize.x + 1];
    }
    private static void InitPathFindingInfo()
    {
        int rowsNb = _battlefieldGrid.Length;
        int colsNb = _battlefieldGrid[0].Length;

        _pathFindingInfo = JaggedArrayUtil.InitJaggedArray<PathFindingInfo>(rowsNb, colsNb, () => new PathFindingInfo(rowsNb, colsNb));
        for (int x = 0; x < rowsNb; x++)
            for (int y = 0; y < colsNb; y++)
                _pathFindingInfo[x][y].ComputeDistances(x, y);
    }

    public PathFindingInfo GetPathFindingInfo(int xPos, int yPos)
    {
        return _pathFindingInfo[xPos][yPos];
    }

    private Vector3Int CoordsToTilemapCell(Coords coords)
    {
        return new Vector3Int(coords.y - 1, coords.x, 0);
    }

    public bool MoveUnitTo(Coords curCoords, Coords targetCoords)
    {
        Transform target = _battlefieldGrid[targetCoords.x][targetCoords.y];
        if (target != null)
            return false;

        Transform unitTransform = _battlefieldGrid[curCoords.x][curCoords.y];

        if (!_playerBoardManager.MoveUnitTo(unitTransform, CoordsToTilemapCell(targetCoords)))
        {
            if (!_opponentBoardManager.MoveUnitTo(unitTransform, CoordsToTilemapCell(targetCoords)))
            {
                Debug.LogError("Could not move unit !");
                return false;
            }
        }

        _battlefieldGrid[curCoords.x][curCoords.y] = null;
        _battlefieldGrid[targetCoords.x][targetCoords.y] = unitTransform;
        return true;
    }

    // Careful : the function returns a xPos and yPos but when accessing values in the grid, they should be called with [yPos][xPos]
    public (int, int) ToBattlefieldCoord(Vector3 position)
    {
        return _playerBoardManager.ToBattlefieldCoord(position); // using the playerBoard or the opponentBoard is equivalent
    }

    public void RemoveUnit(Transform deadUnit)
    {
        (int xPos, int yPos) = ToBattlefieldCoord(deadUnit.position);
        _battlefieldGrid[yPos][xPos] = null;
        deadUnit.gameObject.SetActive(false);
        _saveUnits.Add(deadUnit.gameObject);
    }

    public void RemoveUnitAt(Coords unitCoords)
    {
        Transform deadUnit = _battlefieldGrid[unitCoords.x][unitCoords.y];
        _battlefieldGrid[unitCoords.x][unitCoords.y] = null;
        deadUnit.gameObject.SetActive(false);
        _saveUnits.Add(deadUnit.gameObject);
    }

    public void SavePositions()
    {
        _saveUnits = new List<GameObject>();
        _saveBattlefieldGrid = new Transform[_battlefieldGrid.Length][];
        for (int x = 0; x < _battlefieldGrid.Length; x++)
        {
            _saveBattlefieldGrid[x] = new Transform[_battlefieldGrid[x].Length];
            for (int y = 0; y < _battlefieldGrid[x].Length; y++)
            {
                if (_battlefieldGrid[x][y] != null)
                    _battlefieldGrid[x][y].GetComponent<Unit>().SavePosition();
                _saveBattlefieldGrid[x][y] = _battlefieldGrid[x][y];
            }
        }
    }

    public void RestorePositions()
    {
        for (int x = 0; x < _battlefieldGrid.Length; x++)
        {
            for (int y = 0; y < _battlefieldGrid[x].Length; y++)
            {
                if (_battlefieldGrid[x][y] != null) // if the unit was not dead, put it back physically
                    _battlefieldGrid[x][y].GetComponent<Unit>().Reset();
                _battlefieldGrid[x][y] = _saveBattlefieldGrid[x][y]; // set the current cell to what it was before the fight (null or transform)
            }
        }

        foreach (GameObject unit in _saveUnits)
        {
            unit.GetComponent<Unit>().Reset();
            unit.SetActive(true);
        }
    }

    public void DisplayPath(Coords startingCell)
    {
        PathFindingInfo pathFindingInfo = _pathFindingInfo[startingCell.x][startingCell.y];
        PathFindingInfo.HexCellInfo[][] hexCellInfos = pathFindingInfo.GetHexCellInfos();

        for (int x = 0; x < hexCellInfos.Length; x++)
        {
            for (int y = 0; y < hexCellInfos[x].Length; y++)
            {
                if (hexCellInfos[x][y].dist != 0)
                {
                    Coords targetCoords = new Coords(x, y);
                    Vector3Int targetCellPos = CoordsToTilemapCell(targetCoords);
                    Vector3 targetCellCenter = _playerBoardManager.GetCellCenterWorldBattlefield(targetCellPos);

                    Vector3Int fromCellPos = CoordsToTilemapCell(hexCellInfos[x][y].fromCell);
                    Vector3 fromCellCenter = _playerBoardManager.GetCellCenterWorldBattlefield(fromCellPos);

                    Vector3 direction = targetCellCenter - fromCellCenter;
                    Instantiate(arrowHelperPrefab, fromCellCenter, Quaternion.Euler(0, 90, 0) * Quaternion.LookRotation(direction, Vector3.up));
                }
            }
        }
    }
}
