using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Targeting/FarthestEnemy")]
public class FarthestEnemy : AbilityTargetBase
{
    [SerializeField] int nbTargets = 1;
    public override void SetTarget(Transform target)
    {
        _target = null;
    }

    public override List<Unit> GetTargets(Unit caster)
    {
        // Get unit position
        Vector3 pos = caster.transform.position;
    
        BoardManager boardManager = GameManager.Instance.GetBoardManager();
        PvPManager pvpManager = GameManager.Instance.GetPvPManager();
        // Get unit cell
        (int xPos, int yPos) = boardManager.ToBattlefieldCoord(pos);
        // Get distance info from unit cell
        PathFindingInfo pathFindingInfo = boardManager.GetPathFindingInfo(yPos, xPos);

        // Extract the list of opposite team coordinates
        Transform[][] units = boardManager.GetBattlefield();
        (List<Coords> playerCoordsList, List<Coords> opponentCoordsList) = pvpManager.GetUnitsCoords(units); // differentiate player and enemy teams
        List<Coords> targetCoordsList = caster.IsFromPlayerTeam() ? opponentCoordsList : playerCoordsList;

        // Get the nbTargets farthest units from unt
        List<Coords> targetCoords = pvpManager.FindMultipleFarthestCoords(targetCoordsList, pathFindingInfo.GetDistances(), nbTargets);
        // Get transform of all the cells at radius distance from unit 
        List<Transform> targetTransforms = boardManager.GetUnitsAt(targetCoords);

        List<Unit> targets = new List<Unit>();
        foreach (Transform targetTransform in targetTransforms)
        {
            Unit unit = targetTransform.GetComponent<Unit>();
            targets.Add(unit);
        }

        return targets;
    } 
}