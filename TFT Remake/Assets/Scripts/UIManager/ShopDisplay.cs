using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;

public class ShopDisplay
{
    private static ShopDisplay _instance;
    private static UIDocument _uiDoc;

    public static ShopDisplay Instance(UIDocument uiDoc)
    {
        if (_instance == null)
        {
            _instance = new ShopDisplay();
            _uiDoc = uiDoc;
        }
        return _instance;
    }
    
    private int MAX_TRAITS_DISPLAYED = 3;
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
    private VisualElement[] _slotsCost;
    private VisualElement[] _slotsImage;
    private Label[] _slotsName;
    private Color[] _costColors;
    private Label[][] _slotsTraitName;
    private VisualElement[][] _slotsTraitTextures;

    private T GetUIElement<T>(string name) where T : UnityEngine.UIElements.VisualElement
    {
        return _uiDoc.rootVisualElement.Q<T>(name);
    }

    private void InitShopSlots()
    {
        int shop_size = GameManager.Instance.GetShopManager().SHOP_SIZE;

        _slotsCost = new VisualElement[shop_size];
        _slotsImage = new VisualElement[shop_size];
        _slotsName = new Label[shop_size];

        _slotsTraitName = new Label[shop_size][];
        _slotsTraitTextures = new VisualElement[shop_size][];
        for (int i = 0; i < shop_size; i++)
        {
            int slotIndex = i + 1;
            _slotsCost[i] = GetUIElement<VisualElement>($"Slot{slotIndex}Cost");
            _slotsImage[i] = GetUIElement<VisualElement>($"Slot{slotIndex}Unit");
            _slotsName[i] = GetUIElement<Label>($"Slot{slotIndex}UnitName");

            _slotsTraitName[i] = new Label[MAX_TRAITS_DISPLAYED];
            _slotsTraitTextures[i] = new VisualElement[MAX_TRAITS_DISPLAYED];
            for (int j = 0; j < MAX_TRAITS_DISPLAYED; j++)
            {
                int traitIndex = j + 1;
                _slotsTraitName[i][j] = GetUIElement<Label>($"Slot{slotIndex}Trait{traitIndex}");
                _slotsTraitTextures[i][j] = GetUIElement<VisualElement>($"Slot{slotIndex}Trait{traitIndex}");
            }
        }
    }

    private void InitCostColors()
    {
        _costColors = new Color[3];
        ColorUtility.TryParseHtmlString("#96A194", out _costColors[0]);
        ColorUtility.TryParseHtmlString("#40BBDD", out _costColors[1]);
        ColorUtility.TryParseHtmlString("#FA9607", out _costColors[2]);
    }

    public void InitShopDisplay()
    {
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

        InitShopSlots();
        InitCostColors();

        _unitShop.visible = false;
    }
    public void UpdateGold(int gold)
    {
        _gold.text = $"{gold}";
    }
    
    public void UpdateXP(int xp, int totalXP)
    {
        _xp.text = $"{xp}";
        _totalXP.text = $"{totalXP}";
    }

    public void UpdateLevel(int level, int maxLevel)
    {
        _lvl.text = $"{level}";

        if (level != 1)
            _unitShop.visible = true; // shop is not visible during the very first round/level
        if (level == maxLevel)
            _currentXP.visible = false;

        (float oneCostPer, float threeCostPer, float fiveCostPer) = GameManager.Instance.GetShopManager().GetPoolPercentage(level);
        _1costPoolPercentage.text = $"{oneCostPer*100}%";
        _3costPoolPercentage.text = $"{threeCostPer*100}%";
        _5costPoolPercentage.text = $"{fiveCostPer*100}%";
    }

    private void DisplayTraits(int index, UnitType unitType)
    {
        GameObject unitGO = GameManager.Instance.GetShopManager().GetUnitFromUnitType(unitType);
        Trait[] traits = unitGO.GetComponent<Unit>().stats.traits;

        for (int i = 0; i < _slotsTraitTextures[index].Length; i++)
        {
            if (i < traits.Length
                && traits[i] != Trait.None)
            {
                Texture2D tex = Resources.Load<Texture2D>(TraitUtil.ToTexture(traits[i]));
                _slotsTraitTextures[index][i].style.backgroundImage = tex;
                _slotsTraitName[index][i].text = TraitUtil.ToString(traits[i]);
                _slotsTraitTextures[index][i].visible = true;
            }
            else
                _slotsTraitTextures[index][i].visible = false;
        }
    }

    public void UpdateShop(UnitType[] shop)
    {
        for (int i = 0; i < shop.Length; i++)
        {
            UnitType unitType = shop[i];
            _slotsName[i].text = unitType.ToString();
            _slotsImage[i].style.backgroundImage = Resources.Load<Texture2D>($"{unitType.ToString()}"); ;
            int costIndex = GameManager.Instance.GetShopManager().GetUnitCostIndexFromUnitType(unitType);
            if (costIndex != -1) // unit is not a target dummy
                _slotsCost[i].style.backgroundColor = _costColors[costIndex];
            else
                _slotsCost[i].style.backgroundColor = new Color(0, 0, 0);

            DisplayTraits(i, unitType);
        }
    }

    public void UpdateXPCostDisplay(int xpCost)
    {
        _xpCost.text = $"{xpCost}";
    }


    // public void ShowUnitDisplay(Transform unitTransform)
    // {
    //     Unit unit = unitTransform.GetComponent<Unit>();
    //     UnitStats stats = unit.stats;

    //     DisplayTraits(_traitTextures, _traitLabels, stats.traits);

    //     _unitImage.style.backgroundImage = Resources.Load<Texture2D>($"{stats.type.ToString()}");;
    //     _name.text = stats.type.ToString();
    //     _name.style.backgroundColor = _costColors[(int)stats.cost];

    //     float shield = unit.GetShield();
    //     float maxHealth = unit.GetMaxHealth() + shield;

    //     float shieldRatio = (unit.GetHealth() + shield) / maxHealth;
    //     float shieldPercent = Mathf.Lerp(0, 100, shieldRatio);
    //     _shieldBarMask.style.width = Length.Percent(shieldPercent);

    //     _healthLabel.text = $"{Mathf.Round(unit.GetHealth())}/{Mathf.Round(unit.GetMaxHealth())}";
    //     float healthRatio = unit.GetHealth() / maxHealth;
    //     float healthPercent = Mathf.Lerp(0, 100, healthRatio);
    //     _healthBarMask.style.width = Length.Percent(healthPercent);

    //     _manaLabel.text = $"{Mathf.Round(unit.GetMana())}/{stats.mana[1]}";
    //     float manaRatio = unit.GetMana() / stats.mana[1];
    //     float manaPercent = Mathf.Lerp(0, 100, manaRatio);
    //     _manaBarMask.style.width = Length.Percent(manaPercent);

    //     _ap.text = $"{unit.GetAP()}%";
    //     _ad.text = $"{unit.GetAD()}%";
    //     _baseDamage.text = $"{stats.attackDamage[(int)stats.star]}";
    //     _armor.text = $"{unit.GetArmor()}";
    //     _mr.text = $"{unit.GetMR()}";
    //     _atkSpeed.text = $"{unit.GetAS()}";
    //     _crit.text = $"{unit.GetCritChance()}%";
    //     _range.text = $"{unit.GetRange()}";
    //     _dr.text = $"{unit.GetDurability() * 100.0f}%";

    //     // _unitArt.material = unit.GetComponent<Renderer>().material;
    //     _unitDisplayBackground.visible = true;
    // }

    // public void HideUnitDisplay()
    // {
    //     UIUtil.HideVisualElements(_traitTextures);
    //     _unitDisplayBackground.visible = false;
    // }
}
