using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }
    BoardManager _boardManager;
    UIManager _uiManager;
    PvPManager _pvpManager;
    public bool isPlayer;
    Dictionary<Trait, List<Transform>> _synergies = new Dictionary<Trait, List<Transform>>();
    [SerializeField] Camera opponentCamera;
    Camera playerCamera;

    public UnitTraitSO[] traits;

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
        if (traits.Length != Enum.GetNames(typeof(Trait)).Length - 1) // -1 because we don't want None to be taken int account
            Debug.LogError("There is not the correct number of UnitTraitSO in the trait attribute.");
        Init();
    }

    bool FindManager<T>(ref T manager) where T : UnityEngine.Object
    {
        T[] managers = FindObjectsByType<T>(FindObjectsSortMode.None);
        if (managers == null || managers.Length == 0)
        {
            Debug.LogError("Could not find any " + typeof(T).Name + " script.");
            return false;
        }
        else if (managers.Length > 1)
            Debug.Log("Multiple " + typeof(T).Name + " scripts have been found.");
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

        bool findPVPManager = FindManager<PvPManager>(ref _pvpManager);
        if (findPVPManager)
            _pvpManager.Init();

        isPlayer = true;

        playerCamera = Camera.main;
        playerCamera.enabled = true;
        playerCamera.GetComponent<DragAndDrop>().enabled = true;
        opponentCamera.enabled = false;
        opponentCamera.GetComponent<DragAndDrop>().enabled = false;
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
        Trait[] traits = unitStats.traits;

        bool isSynergiesModified = false;

        foreach (Trait trait in traits)
        {
            if (trait == Trait.None) // None trait is skipped because not considered to be an actual trait
                continue;

            if (!_synergies.ContainsKey(trait)) // the dictionnary does not have this trait yet
            {
                if (eventArgs.toZone == MoveUnitEventArgs.Zone.Battlefield) // the unit is added to the battlefield
                {
                    List<Transform> units = new List<Transform>();
                    units.Add(unit);
                    _synergies.Add(trait, units);
                    isSynergiesModified = true;
                }
                // else, do nothing
            }
            else if (!_synergies[trait].Contains(unit) // the dictionnary have the trait but the unit is not in the list
                     && eventArgs.toZone == MoveUnitEventArgs.Zone.Battlefield) // AND is added to the battlefield
            {
                _synergies[trait].Add(unit);
                isSynergiesModified = true;
            }
            else if (eventArgs.toZone == MoveUnitEventArgs.Zone.Bench // the unit is move to the bench
                     && _synergies[trait].Contains(unit)) // AND the unit is mentionned in the dictionnary
            {
                _synergies[trait].Remove(unit);
                if (_synergies[trait].Count == 0)
                    _synergies.Remove(trait);
                isSynergiesModified = true;
            }
        }

        if (isSynergiesModified)
            UpdateSynergyDisplay();
    }

    public void UpdateSynergyDisplay()
    {
        _uiManager.UpdateSynergyDisplay(_synergies, traits);
    }

    public void Fight()
    {
        _pvpManager.Fight(_boardManager.GetBattlefield());
    }

    public void ChangePlayer()
    {
        isPlayer = !isPlayer;
        opponentCamera.enabled = !opponentCamera.enabled;
        opponentCamera.GetComponent<DragAndDrop>().enabled = !opponentCamera.GetComponent<DragAndDrop>().enabled;
        playerCamera.enabled = !playerCamera.enabled;
        playerCamera.GetComponent<DragAndDrop>().enabled = !playerCamera.GetComponent<DragAndDrop>().enabled;

        _uiManager.ChangePlayer(isPlayer);
    }

    private void DumpSyergies()
    {
        string str = "[";
        foreach (KeyValuePair<Trait, List<Transform>> kvp in _synergies)
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
