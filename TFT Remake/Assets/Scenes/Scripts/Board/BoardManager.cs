using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class BoardManager : MonoBehaviour
{
    private float MIN_BATTLEFIELD_Z = 0f;
    private float MAX_BATTLEFIELD_Z = 5.25f;
    Vector3 _initUnitPos;
    Tilemap _playerBattlefield;
    Tilemap _playerBench;

    Transform[][] _battlefield;
    Transform[][] _bench;

    void Awake()
    {
        _playerBattlefield = null;
        _playerBench = null;
    }

    void Start()
    {
        _initUnitPos = Vector3.zero;
        Tilemap[] tilemaps = gameObject.GetComponentsInChildren<Tilemap>();
        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap.CompareTag("Player Battlefield"))
                _playerBattlefield = tilemap;
            else if (tilemap.CompareTag("Player Bench"))
                _playerBench = tilemap;
        }

        InitBoard(_playerBattlefield, ref _battlefield, false);
        InitBoard(_playerBench, ref _bench, true);
    }

    public void OnDragUnit(Transform unitTransform)
    {
        _initUnitPos = unitTransform.position;
    }

    public void OnDropUnit(Transform unitTransform)
    {
        if (unitTransform == null)
            return;

        Vector3 unitPos = new Vector3(unitTransform.position.x, _initUnitPos.y, unitTransform.position.z);
        if (!DropOnZone(unitTransform, unitPos, _playerBattlefield))
        {
            if (!DropOnZone(unitTransform, unitPos, _playerBench))
                unitTransform.position = _initUnitPos;
        }
    }

    private bool DropOnZone(Transform unitTransform, Vector3 unitPos, Tilemap boardZone)
    {
        Vector3Int cellPos = boardZone.WorldToCell(unitPos);

        if (boardZone.cellBounds.Contains(cellPos) && boardZone.HasTile(cellPos))
        {
            Vector3 cellCenterPos = boardZone.GetCellCenterWorld(cellPos);
            unitTransform.position = new Vector3(cellCenterPos.x, _initUnitPos.y, cellCenterPos.z);
            PlaceUnitOnBoard(unitTransform, cellPos, boardZone);
            return true;
        }
        return false;
    }

    // Careful : the battlefield cell width coords go from -1 to 6 so the index on the x axis are incremented by 1
    private (int, int) ToBattlefieldCoord(Vector3Int cellCoord)
    {
        return (cellCoord.x + 1, cellCoord.y);
    }

    // Careful : the bench cell width coords go from -1 to 8 so the index on the x axis are incremented by 1
    private (int, int) ToBenchCoord(Vector3Int cellCoord, bool log = false)
    {
        if (log)
            Debug.Log("Cell coord: " + cellCoord + " => (" + (cellCoord.x + 1) + ", " + (cellCoord.y == -1 ? 0 : 1) + ")");
        return (cellCoord.x + 1, cellCoord.y == -1 ? 0 : 1);
    }

    private void PlaceUnitOnBoard(Transform unitTransform, Vector3Int cellPos, Tilemap boardZone)
    {
        // assess init unit zone depending on the z coord
        bool isInitUnitOnBattlefield = _initUnitPos.z >= MIN_BATTLEFIELD_Z && _initUnitPos.z <= MAX_BATTLEFIELD_Z;

        // get cell coords of the init position of the dropped unit, depending on the zone (battlefield of bench)
        Vector3Int initUnitCell = isInitUnitOnBattlefield ? _playerBattlefield.WorldToCell(_initUnitPos) : _playerBench.WorldToCell(_initUnitPos);
        (int xInitCellPos, int yInitCellPos) = isInitUnitOnBattlefield ? ToBattlefieldCoord(initUnitCell) : ToBenchCoord(initUnitCell);

        if (boardZone == _playerBattlefield)
        {
            (int xPos, int yPos) = ToBattlefieldCoord(cellPos);
            // get unit on the drop cell
            Transform swapUnitTransform = _battlefield[yPos][xPos];
            // set cell of the dropped unit to the one on the drop cell
            if (isInitUnitOnBattlefield)
                _battlefield[yInitCellPos][xInitCellPos] = swapUnitTransform;
            else
                _bench[yInitCellPos][xInitCellPos] = swapUnitTransform;

            if (swapUnitTransform != null)
                swapUnitTransform.position = _initUnitPos; // if the unit of the dropped cell exist, move its position

            _battlefield[yPos][xPos] = unitTransform; // move the dropped unit on the drop cell
        }
        else
        {
            (int xPos, int yPos) = ToBenchCoord(cellPos);
            Transform swapUnitTransform = _bench[yPos][xPos];
            if (isInitUnitOnBattlefield)
                _battlefield[yInitCellPos][xInitCellPos] = swapUnitTransform;
            else
                _bench[yInitCellPos][xInitCellPos] = swapUnitTransform;

            if (swapUnitTransform != null)
                swapUnitTransform.position = _initUnitPos;
            _bench[yPos][xPos] = unitTransform;
        }
        // DumpBoard(_battlefield);
        // DumpBoard(_bench);
    }

    private void InitBoard(Tilemap tilemap, ref Transform[][] board, bool isBench)
    {
        BoundsInt bounds = tilemap.cellBounds;
        Vector3Int boardSize = bounds.size + bounds.position;
        board = new Transform[isBench ? 2 : boardSize.y][];
        for (int i = 0; i < board.Length; i++)
            board[i] = new Transform[boardSize.x + 1];
    }
    private void DumpBoard(Transform[][] board)
    {
        string str = "[";
        foreach (var row in board)
        {
            str += "[";
            int i = 0;
            for (; i < row.Length - 1; i++)
                str += row[i] + ", ";
            str += row[i];
            str += "]\n";
        }
        Debug.Log(str + "]");
    }
}
