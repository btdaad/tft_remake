using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class BoardManager : MonoBehaviour
{
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

    private bool DropOnZone(Transform unitTransform, Vector3 unitPos, Tilemap zone)
    {
        Vector3Int cellPos = zone.WorldToCell(unitPos);

        if (zone.cellBounds.Contains(cellPos) && zone.HasTile(cellPos))
        {
            Vector3 cellCenterPos = zone.GetCellCenterWorld(cellPos);
            unitTransform.position = new Vector3(cellCenterPos.x, _initUnitPos.y, cellCenterPos.z);
            PlaceOnBoard(unitTransform, cellPos, zone);
            return true;
        }
        return false;
    }

    // Careful : the battlefield cell width coords go from -1 to 6 so the index of the 2nd dimension are incremented by 1
    // Careful : the bench cell width coords go from -1 to 8 so the index of the 2nd dimension are incremented by 1

    // TODO : cannot go from bench to battlefield /!\ for player, bench z is < 0.
    private void PlaceOnBoard(Transform unitTransform, Vector3Int cellPos, Tilemap zone)
    {
        // get cell coords of the init position of the dropped unit
        Vector3Int initUnitCell = zone.WorldToCell(_initUnitPos);
        if (zone == _playerBattlefield)
        {
            int yPos = cellPos.y;
            int xPos = cellPos.x + 1;

            // get unit on the drop cell
            Transform swapUnitTransform = _battlefield[yPos][xPos];
            // set battlefield cell of the dropped unit to the one on the drop cell
            _battlefield[initUnitCell.y][initUnitCell.x + 1] = swapUnitTransform;
            if (swapUnitTransform != null)
                swapUnitTransform.position = _initUnitPos; // if the unit of the dropped cell exist, move its position
            _battlefield[yPos][xPos] = unitTransform; // move the dropped unit on the drop cell
            DumpBoard(_battlefield);
        }
        else
        {
            int yPos = cellPos.y == -1 ? 0 : 1;
            int xPos = cellPos.x + 1;

            Transform swapUnitTransform = _bench[yPos][xPos];
            _bench[initUnitCell.y == -1 ? 0 : 1][initUnitCell.x + 1] = swapUnitTransform;
            if (swapUnitTransform != null)
                swapUnitTransform.position = _initUnitPos;
            _bench[yPos][xPos] = unitTransform;
            DumpBoard(_bench);
        }
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
