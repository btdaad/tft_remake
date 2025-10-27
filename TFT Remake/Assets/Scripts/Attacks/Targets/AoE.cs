using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Targeting/AoE")]
public class AoE : AbilityTargetBase
{
    [SerializeField] int radius;
    [SerializeField] bool centeredOnCaster = true; // otherwise centered on target
    private Transform _target;
    public void SetTarget(Transform target)
    {
        _target = centeredOnCaster ? null : target;
    }

    public override List<Unit> GetTargets(Unit caster)
    {
        // Get unit position
        Vector3 pos = caster.transform.position;
        if (!centeredOnCaster)
            pos = _target.position;
    
        BoardManager boardManager = GameManager.Instance.GetBoardManager();
        // Get unit cell
        (int xPos, int yPos) = boardManager.ToBattlefieldCoord(pos);
        // Get distance info from unit cell
        PathFindingInfo pathFindingInfo = boardManager.GetPathFindingInfo(yPos, xPos);
        // Get coords of all the cells up to radius distance from center cell 
        List<Coords> targetCoords = pathFindingInfo.GetCellsTo(radius);
        // Get transform of all the cells at radius distance from unit 
        List<Transform> targetTransforms = boardManager.GetUnitsAt(targetCoords);

        List<Unit> targets = new List<Unit>();
        bool affiliation = caster.IsFromPlayerTeam();
        foreach (Transform targetTransform in targetTransforms)
        {
            Unit unit = targetTransform.GetComponent<Unit>();
            if (unit.IsFromPlayerTeam() != affiliation)
                targets.Add(unit);
        }

        return targets;
    } 
}