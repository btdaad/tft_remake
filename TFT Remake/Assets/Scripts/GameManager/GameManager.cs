using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    BoardManager _boardManager;
    Dictionary<UnitStats.Trait, List<Transform>> synergies = new Dictionary<UnitStats.Trait, List<Transform>>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        BoardManager[] boardManagers = FindObjectsByType<BoardManager>(FindObjectsSortMode.None);
        if (boardManagers == null || boardManagers.Length == 0)
        {
            Debug.LogError("Could not find any Board Manager script.");
            return;
        }
        else if (boardManagers.Length > 1)
            Debug.Log("Multiple Board Manager scripts have been found.");
        _boardManager = boardManagers[0];
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateSynergies(object sender, EventArgs e)
    {
        MoveUnitEventArgs eventArgs = (MoveUnitEventArgs) e;
        Transform unit = eventArgs.unit;
        UnitStats unitStats = unit.GetComponent<UnitStats>();
        UnitStats.Trait[] unitTraits = unitStats.traits;

        // only to ADD units to synergies
        foreach (UnitStats.Trait trait in unitTraits)
        {
            if (!synergies.ContainsKey(trait))
            {
                List<Transform> units = new List<Transform>();
                units.Add(unit);
                synergies.Add(trait, units);
            }
            else if (!synergies[trait].Contains(unit))
                synergies[trait].Add(unit);
        }
    }
}
