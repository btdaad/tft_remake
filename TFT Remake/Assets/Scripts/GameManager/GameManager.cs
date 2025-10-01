using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }
    BoardManager _boardManager;
    Dictionary<UnitStats.Trait, List<Transform>> synergies = new Dictionary<UnitStats.Trait, List<Transform>>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
            _instance = this;
        Init();
    }

    void Init()
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
        _boardManager.Init();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateSynergies(object sender, EventArgs e)
    {
        MoveUnitEventArgs eventArgs = (MoveUnitEventArgs)e;
        Transform unit = eventArgs.unit;
        UnitStats unitStats = unit.GetComponent<Unit>().stats;
        UnitStats.Trait[] unitTraits = unitStats.traits;

        foreach (UnitStats.Trait trait in unitTraits)
        {
            if (trait == UnitStats.Trait.None) // None trait is skipped because not considered to be an actual trait
                continue;

            if (!synergies.ContainsKey(trait)) // the dictionnary does not have this trait yet
            {
                if (eventArgs.toZone == MoveUnitEventArgs.Zone.Battlefield) // the unit is added to the battlefield
                {
                    List<Transform> units = new List<Transform>();
                    units.Add(unit);
                    synergies.Add(trait, units);
                }
                // else, do nothing
            }
            else if (!synergies[trait].Contains(unit) // the dictionnary have the trait but the unit is not in the list
                     && eventArgs.toZone == MoveUnitEventArgs.Zone.Battlefield) // AND is added to the battlefield
                synergies[trait].Add(unit);
            else if (eventArgs.toZone == MoveUnitEventArgs.Zone.Bench // the unit is move to the bench
                     && synergies[trait].Contains(unit)) // AND the unit is mentionned in the dictionnary
            {
                synergies[trait].Remove(unit);
                if (synergies[trait].Count == 0)
                    synergies.Remove(trait);
            }
        }
    }

    private void DumpSyergies()
    {
        string str = "[";
        foreach (KeyValuePair<UnitStats.Trait, List<Transform>> kvp in synergies)
        {
            str += kvp.Key.ToString();
            str += ": (";
            foreach (Transform unit in kvp.Value)
            {
                str += unit;
                str += " ";
            }
            str += ")\n";
        }
        str += "]";
        Debug.Log(str);
    }
}
