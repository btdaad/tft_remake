using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }
    BoardManager _boardManager;
    UIManager _uiManager;
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

    bool FindManager<T>(ref T manager) where T : UnityEngine.Object
    {
        T[] managers = FindObjectsByType<T>(FindObjectsSortMode.None);
        if (managers == null || managers.Length == 0)
        {
            Debug.LogError("Could not find any " + nameof(T) + " script.");
            return false;
        }
        else if (managers.Length > 1)
            Debug.Log("Multiple " + nameof(T) + " scripts have been found.");
        manager = managers[0];
        return true;
    }

    void Init()
    {
        bool findBoardManager = FindManager<BoardManager>(ref _boardManager);
        if (findBoardManager)
            _boardManager.Init();

        bool findUIManager = FindManager<UIManager>(ref _uiManager);
        if (findUIManager)
            _uiManager.Init();
    }
    public BoardManager GetBoardManager()
    {
        return _boardManager;
    }
    
    public UIManager GetUIManager()
    {
        return _uiManager;
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
