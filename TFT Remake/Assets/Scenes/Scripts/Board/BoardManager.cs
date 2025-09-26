using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    Vector3 _initUnitPos;
    Tilemap _playerBattlefield;
    Tilemap _playerBench;

    Transform[,] _battlefield;
    Transform[,] _bench;

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
        _battlefield = new Transform[battlefieldSize.x, battlefieldSize.y];

        BoundsInt benchBounds = _playerBench.cellBounds;
        Vector3Int benchSize = benchBounds.size + benchBounds.position;
        _bench = new Transform[benchSize.x, 2];
        
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
        Debug.Log("Cell Bounds: " + zone.cellBounds);

        if (zone.cellBounds.Contains(cellPos) && zone.HasTile(cellPos))
        {
            Debug.Log("Cell Pos: " + cellPos);
            Vector3 cellCenterPos = zone.GetCellCenterWorld(cellPos);
            unitTransform.position = new Vector3(cellCenterPos.x, _initUnitPos.y, cellCenterPos.z);
            return true;
        }
        return false;
    }
}
