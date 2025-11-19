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
    private Label _refreshCost;
    private VisualElement _unitShop;
    private VisualElement _currentXP;
    private Label _1costPoolPercentage;
    private Label _3costPoolPercentage;
    private Label _5costPoolPercentage;
    private Button[] _slots;
    private VisualElement[] _slotsWindow;
    private Label[] _slotsCost;
    private VisualElement[] _slotsImage;
    private Label[] _slotsName;
    private Color[] _costColors;
    private Color _hoverBackgroundColorDelta = new Color(0.02f, 0.02f, 0.02f);
    private Label[][] _slotsTraitName;
    private VisualElement[][] _slotsTraitTextures;
    private int[] _unitCost = {1, 3, 5}; // price of units {1, 3, 5}

    private T GetUIElement<T>(string name) where T : UnityEngine.UIElements.VisualElement
    {
        return _uiDoc.rootVisualElement.Q<T>(name);
    }

    private void InitShopSlots()
    {
        int shop_size = GameManager.Instance.GetShopManager().GetShopSize();

        _slots = new Button[shop_size];
        _slotsWindow = new VisualElement[shop_size];
        _slotsImage = new VisualElement[shop_size];
        _slotsName = new Label[shop_size];
        _slotsCost = new Label[shop_size];

        _slotsTraitName = new Label[shop_size][];
        _slotsTraitTextures = new VisualElement[shop_size][];
        for (int i = 0; i < shop_size; i++)
        {
            int slotIndex = i + 1;
            _slotsWindow[i] = GetUIElement<VisualElement>($"Slot{slotIndex}Window");
            _slotsImage[i] = GetUIElement<VisualElement>($"Slot{slotIndex}Unit");
            _slotsName[i] = GetUIElement<Label>($"Slot{slotIndex}UnitName");
            _slotsCost[i] = GetUIElement<Label>($"Slot{slotIndex}Cost");

            _slotsTraitName[i] = new Label[MAX_TRAITS_DISPLAYED];
            _slotsTraitTextures[i] = new VisualElement[MAX_TRAITS_DISPLAYED];
            for (int j = 0; j < MAX_TRAITS_DISPLAYED; j++)
            {
                int traitIndex = j + 1;
                _slotsTraitName[i][j] = GetUIElement<Label>($"Slot{slotIndex}Trait{traitIndex}");
                _slotsTraitTextures[i][j] = GetUIElement<VisualElement>($"Slot{slotIndex}Trait{traitIndex}");
            }
            
            _slots[i] = GetUIElement<Button>($"Slot{slotIndex}");
            _slots[i].RegisterCallback<ClickEvent, int>(BuyUnit, i);
            _slots[i].RegisterCallback<MouseOverEvent, int>(HoverUnit, i);;
            _slots[i].RegisterCallback<MouseOutEvent, int>(LeaveUnit, i);;
        }
    }

    private void BuyUnit(ClickEvent _, int i)
    {
        GameManager.Instance.BuyUnit(i);
    }
    
    private void HoverUnit(MouseOverEvent _, int i)
    {
        _slotsWindow[i].style.backgroundColor = _slotsWindow[i].style.backgroundColor.value + _hoverBackgroundColorDelta;
    }
    private void LeaveUnit(MouseOutEvent _, int i)
    {
        _slotsWindow[i].style.backgroundColor = _slotsWindow[i].style.backgroundColor.value - _hoverBackgroundColorDelta;
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
        _refreshCost = GetUIElement<Label>("RefreshCost");

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
            {
                _slots[i].visible = true;
                _slotsWindow[i].style.backgroundColor = _costColors[costIndex];
                _slotsCost[i].text = $"{_unitCost[costIndex]}";
            }
            else
                _slots[i].visible = false;

            DisplayTraits(i, unitType);
        }
    }

    public void UpdateXPCostDisplay(int xpCost)
    {
        _xpCost.text = $"{xpCost}";
    }
    
    public void UpdateRefreshCostDisplay(int refreshCost)
    {
        _refreshCost.text = $"{refreshCost}";
    }
    public void UpdateUnitCostDisplay(int[] diffCost)
    {
        for (int i = 0; i < _slotsCost.Length; i++)
        {
            int cost = Int32.Parse(_slotsCost[i].text);
            int index = Array.IndexOf(_unitCost, cost);
            if (index != -1)
                _slotsCost[i].text = $"{diffCost[index]}";
            // if index == -1, it could be seen as an error but truthfully, this code is called before the shop open so the prices are not correct at first anyway
        }
        _unitCost = (int[]) diffCost.Clone(); // need to clone otherwise it is a deep copy and it is updated as soon as diffCost is updated
    }
}
