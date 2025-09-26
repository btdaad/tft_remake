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

        BoundsInt battlefieldBounds = _playerBattlefield.cellBounds;
        Vector3Int battlefieldSize = battlefieldBounds.size + battlefieldBounds.position;
        _battlefield = new Transform[battlefieldSize.y][];
        for (int i = 0; i < battlefieldSize.y; i++)
            _battlefield[i] = new Transform[battlefieldSize.x];
        Debug.Log("_battlefield[" + battlefieldSize.y + "][" + battlefieldSize.x + "]");
        DumpBoard(_battlefield);

        BoundsInt benchBounds = _playerBench.cellBounds;
        Vector3Int benchSize = benchBounds.size + benchBounds.position;
        _bench = new Transform[2][];
        for (int i = 0; i < 2; i++)
            _bench[i] = new Transform[benchSize.x];
        Debug.Log("_bench[" + 2 + "][" + benchSize.x + "]");
        DumpBoard(_bench);
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

    private void PlaceOnBoard(Transform unitTransform, Vector3Int cellPos, Tilemap zone)
    {
        if (zone == _playerBattlefield)
            _battlefield[cellPos.y + 1][cellPos.x + 1] = unitTransform;
        else
            _bench[cellPos.y == -1 ? 0 : 1][cellPos.x + 1] = unitTransform;

        DumpBoard(_battlefield);
        DumpBoard(_bench);
    }

    private void DumpBoard(Transform[][] board)
    {
        string str = "[";
        foreach (var row in board)
        {
            str += "[";
            foreach (var cell in row)
                str += cell + ", ";
            str += "]\n";
        }
        Debug.Log(str + "]");
    }
}
