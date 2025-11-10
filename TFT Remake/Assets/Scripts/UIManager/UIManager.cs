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
    private Button _refreshShop;
    private Label _xpCost;
    private VisualElement _unitShop;
    private VisualElement _currentXP;
    private Label _1costPoolPercentage;
    private Label _3costPoolPercentage;
    private Label _5costPoolPercentage;
    
    private T GetUIElement<T>(string name) where T : UnityEngine.UIElements.VisualElement
    {
        return UIDoc.rootVisualElement.Q<T>(name);
    }

    public void Init()
    {
        UIUnit = UnitsDisplay.Instance(UIDoc);
        UIUnit.InitUnitDisplay();
        UISynergy = SynergiesDisplay.Instance(UIDoc);
        UISynergy.InitSynergyDisplay();

        _fight = GetUIElement<Button>("Fight");
        _fight.clickable.clicked += () => { GameManager.Instance.Fight(); };
        _changePlayer = GetUIElement<Button>("ChangePlayer");
        _changePlayer.clickable.clicked += () => { GameManager.Instance.ChangePlayer(); };
        Button dumpButton = GetUIElement<Button>("Dump");
        dumpButton.clickable.clicked += () => { GameManager.Instance.Dump(); };

        _unitShop = GetUIElement<VisualElement>("UnitShop");
        _currentXP = GetUIElement<VisualElement>("CurrentXP");
        _gold = GetUIElement<Label>("Gold");
        _lvl = GetUIElement<Label>("LevelNb");
        _xp = GetUIElement<Label>("XP");
        _totalXP = GetUIElement<Label>("TotalXP");
        _xpCost = GetUIElement<Label>("XPCost");
        _buyXP = GetUIElement<Button>("BuyXP");
        _buyXP.clickable.clicked += () => { GameManager.Instance.BuyXP(); };
        _refreshShop = GetUIElement<Button>("Refresh");
        _refreshShop.clickable.clicked += () => { GameManager.Instance.RefreshShop(); };
        _1costPoolPercentage = GetUIElement<Label>("1CostPercentage");
        _3costPoolPercentage = GetUIElement<Label>("3CostPercentage");
        _5costPoolPercentage = GetUIElement<Label>("5CostPercentage");

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

        (float oneCostPer, float threeCostPer, float fiveCostPer) = GameManager.Instance.GetShopManager().GetPoolPercentage(level);
        _1costPoolPercentage.text = $"{oneCostPer*100}%";
        _3costPoolPercentage.text = $"{threeCostPer*100}%";
        _5costPoolPercentage.text = $"{fiveCostPer*100}%";
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
