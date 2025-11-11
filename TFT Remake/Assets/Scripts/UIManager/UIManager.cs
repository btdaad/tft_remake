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
    private ShopDisplay UIShop;
    private Button _fight;
    private Button _changePlayer;
    
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
        UIShop = ShopDisplay.Instance(UIDoc);
        UIShop.InitShopDisplay();

        _fight = GetUIElement<Button>("Fight");
        _fight.clickable.clicked += () => { GameManager.Instance.Fight(); };
        _changePlayer = GetUIElement<Button>("ChangePlayer");
        _changePlayer.clickable.clicked += () => { GameManager.Instance.ChangePlayer(); };
        Button dumpButton = GetUIElement<Button>("Dump");
        dumpButton.clickable.clicked += () => { GameManager.Instance.Dump(); };
    }

    public void UpdateSynergyDisplay(Dictionary<Trait, List<Transform>> unsortedSynergies, UnitTraitSO[] unitTraits)
    {
        UISynergy.UpdateSynergyDisplay(unsortedSynergies, unitTraits);
    }

    public void UpdateGold(bool isPlayer)
    {
        int gold = GameManager.Instance.GetGoldManager().GetGold(isPlayer);
        UIShop.UpdateGold(gold);
    }

    public void UpdateXP(bool isPlayer)
    {
        int xp = GameManager.Instance.GetXPManager().GetXP(isPlayer);
        int totalXP = GameManager.Instance.GetXPManager().GetTotalXP(isPlayer);
        UIShop.UpdateXP(xp, totalXP);
    }

    public void UpdateLevel(bool isPlayer)
    {
        int level = GameManager.Instance.GetXPManager().GetLevel(isPlayer);
        int maxLevel = GameManager.Instance.GetXPManager().GetMaxLevel();
        UIShop.UpdateLevel(level, maxLevel);
    }

    public void UpdateShop(bool isPlayer)
    {
        UnitType[] shop = GameManager.Instance.GetShopManager().GetShop(isPlayer);
        UIShop.UpdateShop(shop);
    }

    public void ShowUnitDisplay(Transform unitTransform)
    {
        UIUnit.ShowUnitDisplay(unitTransform);
    }

    public void HideUnitDisplay()
    {
        UIUnit.HideUnitDisplay();
    }

    // Used to have a responsive ui display of the xp cost if ever it is changed in the Editor
    void Update()
    {
        int xpCost = GameManager.Instance.GetXPManager().GetXPCost();
        UIShop.UpdateXPCostDisplay(xpCost);
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
        UpdateShop(isPlayer);
    }
}
