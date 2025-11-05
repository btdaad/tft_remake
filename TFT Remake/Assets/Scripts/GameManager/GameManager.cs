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
    GoldManager _goldManager;
    public bool isPlayer;
    Dictionary<Trait, List<Transform>> _playerSynergies = new Dictionary<Trait, List<Transform>>();
    Dictionary<Trait, List<Transform>> _opponentSynergies = new Dictionary<Trait, List<Transform>>();
    [SerializeField] Camera opponentCamera;
    Camera playerCamera;
    Player _player;
    Player _opponent;

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

        bool findGoldManager = FindManager<GoldManager>(ref _goldManager);
        if (findGoldManager)
            _goldManager.Init();

        isPlayer = true;
        _player = new Player();
        _player.Init();
        _opponent = new Player();
        _opponent.Init();

        playerCamera = Camera.main;
        playerCamera.enabled = true;
        playerCamera.GetComponent<DragAndDrop>().enabled = true;
        playerCamera.GetComponent<DisplayUnitStats>().enabled = true;
        opponentCamera.enabled = false;
        opponentCamera.GetComponent<DragAndDrop>().enabled = false;
        opponentCamera.GetComponent<DisplayUnitStats>().enabled = false;
    }
    public BoardManager GetBoardManager()
    {
        return _boardManager;
    }

    public UIManager GetUIManager()
    {
        return _uiManager;
    }

    public GoldManager GetGoldManager()
    {
        return _goldManager;
    }

    public PvPManager GetPvPManager()
    {
        return _pvpManager;
    }

    public Player GetPlayer(bool isPlayer)
    {
        return isPlayer ? _player : _opponent;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UpdateGoldDisplay()
    {
        _uiManager.UpdateGold(isPlayer);
    }

    public void ManageGold()
    {
        _goldManager.ManageGold(_player, _opponent);
    }

    public void UpdateSynergies(object sender, EventArgs e)
    {
        MoveUnitEventArgs eventArgs = (MoveUnitEventArgs)e;
        Transform unit = eventArgs.unit;
        UnitStats unitStats = unit.GetComponent<Unit>().stats;
        Trait[] traits = unitStats.traits;
        Dictionary<Trait, List<Transform>> synergies = isPlayer ? _playerSynergies : _opponentSynergies;

        bool isSynergiesModified = false;

        foreach (Trait trait in traits)
        {
            if (trait == Trait.None) // None trait is skipped because not considered to be an actual trait
                continue;

            if (!synergies.ContainsKey(trait)) // the dictionnary does not have this trait yet
            {
                if (eventArgs.toZone == MoveUnitEventArgs.Zone.Battlefield) // the unit is added to the battlefield
                {
                    List<Transform> units = new List<Transform>();
                    units.Add(unit);
                    synergies.Add(trait, units);
                    isSynergiesModified = true;
                }
                // else, do nothing
            }
            else if (!synergies[trait].Contains(unit) // the dictionnary have the trait but the unit is not in the list
                     && eventArgs.toZone == MoveUnitEventArgs.Zone.Battlefield) // AND is added to the battlefield
            {
                synergies[trait].Add(unit);
                isSynergiesModified = true;
            }
            else if (eventArgs.toZone == MoveUnitEventArgs.Zone.Bench // the unit is move to the bench
                     && synergies[trait].Contains(unit)) // AND the unit is mentionned in the dictionnary
            {
                synergies[trait].Remove(unit);
                if (synergies[trait].Count == 0)
                    synergies.Remove(trait);
                isSynergiesModified = true;
            }
        }

        if (isSynergiesModified)
            UpdateSynergyDisplay();
    }

    public void UpdateSynergyDisplay()
    {
        _uiManager.UpdateSynergyDisplay(isPlayer ? _playerSynergies : _opponentSynergies, traits);
    }

    public void Fight()
    {
        opponentCamera.GetComponent<DragAndDrop>().enabled = false;
        playerCamera.GetComponent<DragAndDrop>().enabled = false;
        _boardManager.SavePositions();
        _pvpManager.Fight();
    }

    public void EndFight(bool hasPlayerWon, int damage)
    {
        if (hasPlayerWon)
        {
            _opponent.Defeat(damage);
            _player.Victory();
            _goldManager.BonusPvP(_player);
        }
        else
        {
            _player.Defeat(damage);
            _opponent.Victory();
            _goldManager.BonusPvP(_opponent);
        }
    }

    public void Dump()
    {
        JaggedArrayUtil.Dump<Transform>(_boardManager.GetBattlefield());
        JaggedArrayUtil.Dump<int>(_boardManager.GetDistances()[0][1].GetDistances());
        _boardManager.DisplayPath(new Coords(0, 1));
    }

    public void ChangePlayer()
    {
        isPlayer = !isPlayer;
        opponentCamera.enabled = !opponentCamera.enabled;
        opponentCamera.GetComponent<DragAndDrop>().enabled = !opponentCamera.GetComponent<DragAndDrop>().enabled;
        opponentCamera.GetComponent<DisplayUnitStats>().enabled = !opponentCamera.GetComponent<DisplayUnitStats>().enabled;
        playerCamera.enabled = !playerCamera.enabled;
        playerCamera.GetComponent<DragAndDrop>().enabled = !playerCamera.GetComponent<DragAndDrop>().enabled;
        playerCamera.GetComponent<DisplayUnitStats>().enabled = !playerCamera.GetComponent<DisplayUnitStats>().enabled;

        _uiManager.HideUnitDisplay();
        _uiManager.ChangePlayer(isPlayer);
        UpdateSynergyDisplay();
    }

    private void DumpSyergies(Dictionary<Trait, List<Transform>> synergies)
    {
        string str = "[";
        foreach (KeyValuePair<Trait, List<Transform>> kvp in synergies)
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
