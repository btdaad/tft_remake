using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

public class UIManager : MonoBehaviour
{
    [SerializeField] UIDocument UIDoc;

    private UnitsDisplay UIUnit;
    private SynergiesDisplay UISynergy;
    private Button _fight;
    private Button _changePlayer;
    private Label _gold;
    private Label _lvl;
    private Label _xp;
    private Label _totalXP;
    private Button _buyXP;
    private Label _xpCost;
    private VisualElement _unitShop;
    private VisualElement _currentXP;

    public void Init()
    {
        UIUnit = UnitsDisplay.Instance(UIDoc);
        UIUnit.InitUnitDisplay();
        UISynergy = SynergiesDisplay.Instance(UIDoc);
        UISynergy.InitSynergyDisplay();

        _fight = UIDoc.rootVisualElement.Q<Button>("Fight");
        _fight.clickable.clicked += () => { GameManager.Instance.Fight(); };
        _changePlayer = UIDoc.rootVisualElement.Q<Button>("ChangePlayer");
        _changePlayer.clickable.clicked += () => { GameManager.Instance.ChangePlayer(); };
        _gold = UIDoc.rootVisualElement.Q<Label>("Gold");
        Button dumpButton = UIDoc.rootVisualElement.Q<Button>("Dump");
        dumpButton.clickable.clicked += () => { GameManager.Instance.Dump(); };

        _unitShop = UIDoc.rootVisualElement.Q<VisualElement>("UnitShop");
        _currentXP = UIDoc.rootVisualElement.Q<VisualElement>("CurrentXP");
        _lvl = UIDoc.rootVisualElement.Q<Label>("LevelNb");
        _xp = UIDoc.rootVisualElement.Q<Label>("XP");
        _totalXP = UIDoc.rootVisualElement.Q<Label>("TotalXP");
        _xpCost = UIDoc.rootVisualElement.Q<Label>("XPCost");
        _buyXP = UIDoc.rootVisualElement.Q<Button>("BuyXP");
        _buyXP.clickable.clicked += () => { GameManager.Instance.BuyXP(); };

        _unitShop.visible = false;
    }

    public void UpdateSynergyDisplay(Dictionary<Trait, List<Transform>> unsortedSynergies, UnitTraitSO[] unitTraits)
    {
        UISynergy.UpdateSynergyDisplay(unsortedSynergies, unitTraits);
    }

    public void UpdateGold(bool isPlayer)
    {
        _gold.text = $"{GameManager.Instance.GetGoldManager().GetGold(isPlayer)}";
    }

    public void UpdateXP(bool isPlayer)
    {
        _xp.text = $"{GameManager.Instance.GetXPManager().GetXP(isPlayer)}";
        _totalXP.text = $"{GameManager.Instance.GetXPManager().GetTotalXP(isPlayer)}";
    }

    public void UpdateLevel(bool isPlayer)
    {
        int level = GameManager.Instance.GetXPManager().GetLevel(isPlayer);
        _lvl.text = $"{level}";

        if (level != 1)
            _unitShop.visible = true; // shop is not visible during the very first round/level
        if (level == GameManager.Instance.GetXPManager().GetMaxLevel())
            _currentXP.visible = false;
    }

    public void ShowUnitDisplay(Transform unitTransform)
    {
        UIUnit.ShowUnitDisplay(unitTransform);
    }

    public void HideUnitDisplay()
    {
        UIUnit.HideUnitDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        _xpCost.text = $"{GameManager.Instance.GetXPManager().GetXPCost()}";
    }

    public void ChangePlayer(bool isPlayer)
    {
        if (isPlayer)
            _changePlayer.text = "Play as opponent";
        else
            _changePlayer.text = "Play as yourself";
        UpdateGold(isPlayer);
        UpdateXP(isPlayer);
        UpdateLevel(isPlayer);
    }
}
