using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;

public class UnitsDisplay
{
    private static UnitsDisplay _instance;
    private static UIDocument _uiDoc;

    public static UnitsDisplay Instance(UIDocument uiDoc)
    { 
        if (_instance == null)
        {
            _instance = new UnitsDisplay();
            _uiDoc = uiDoc;
        }
        return _instance;
    }
    
    private int MAX_TRAITS_DISPLAYED = 3;
    private VisualElement _unitDisplayBackground;
    private VisualElement _star;
    private VisualElement _unitImage;
    private VisualElement[] _traitTextures;
    private Label[] _traitLabels;
    private Label _name;
    private Label _healthLabel;
    private VisualElement _healthBarMask;
    private VisualElement _shieldBarMask;
    private Label _manaLabel;
    private VisualElement _manaBarMask;
    private Label _baseDamage;
    private Label _ap;
    private Label _ad;
    private Label _armor;
    private Label _mr;
    private Label _atkSpeed;
    private Label _crit;
    private Label _range;
    private Label _dr;
    private Color[] _costColors;

    private T GetUIElement<T>(string name) where T : UnityEngine.UIElements.VisualElement
    {
        return _uiDoc.rootVisualElement.Q<T>(name);
    }

    private void InitTraits(ref VisualElement[] traitTextures, ref Label[] traitLabels)
    {
        traitTextures = new VisualElement[MAX_TRAITS_DISPLAYED];
        traitLabels = new Label[MAX_TRAITS_DISPLAYED];
        for (int i = 0; i < MAX_TRAITS_DISPLAYED; i++)
        {
            int traitIndex = i + 1;
            traitTextures[i] = GetUIElement<VisualElement>($"Trait{traitIndex}");
            traitLabels[i] = GetUIElement<Label>($"Trait{traitIndex}");
        }
    }

    public void InitUnitDisplay()
    {
        _unitDisplayBackground = GetUIElement<VisualElement>("UnitDisplayBackground");

        _star = GetUIElement<VisualElement>("StarLogo");
        InitTraits(ref _traitTextures, ref _traitLabels);
        _unitImage = GetUIElement<VisualElement>("UnitImage");
        _name = GetUIElement<Label>("Name");

        _shieldBarMask = GetUIElement<VisualElement>("ShieldBarMask");
        _healthLabel = GetUIElement<Label>("HealthLabel");
        _healthBarMask = GetUIElement<VisualElement>("HealthBarMask");
        _manaLabel = GetUIElement<Label>("ManaLabel");
        _manaBarMask = GetUIElement<VisualElement>("ManaBarMask");

        _baseDamage = GetUIElement<Label>("BaseDamage");
        _ap = GetUIElement<Label>("AP");
        _ad = GetUIElement<Label>("AD");
        _armor = GetUIElement<Label>("Armor");
        _mr = GetUIElement<Label>("MR");
        _atkSpeed = GetUIElement<Label>("AtkSpeed");
        _crit = GetUIElement<Label>("Crit");
        _range = GetUIElement<Label>("Range");
        _dr = GetUIElement<Label>("DR");
        
        _costColors = new Color[3];
        ColorUtility.TryParseHtmlString("#96A194", out _costColors[0]);
        _costColors[0].a = 0.6f;
        ColorUtility.TryParseHtmlString("#40BBDD", out _costColors[1]);
        _costColors[1].a = 0.6f;
        ColorUtility.TryParseHtmlString("#FA9607", out _costColors[2]);
        _costColors[2].a = 0.6f;

        _unitDisplayBackground.visible = false;
    }

    private void DisplayTraits(VisualElement[] visualElements, Label[] labels, Trait[] traits)
    {
        for (int i = 0; i < visualElements.Length; i++)
        {
            if (i < traits.Length
                && traits[i] != Trait.None)
            {
                Texture2D tex = Resources.Load<Texture2D>(TraitUtil.ToTexture(traits[i]));
                visualElements[i].style.backgroundImage = tex;
                labels[i].text = TraitUtil.ToString(traits[i]);
                visualElements[i].visible = true;
            }
            else
                visualElements[i].visible = false;
        }
    }

    private string GetStarImageName(Star star)
    {
        switch (star)
        {
            case Star.OneStar:
                return "1star";
            case Star.TwoStar:
                return "2stars";
            case Star.ThreeStar:
                return "3stars";
            default:
                return "";
        }
    }

    public void ShowUnitDisplay(Transform unitTransform)
    {
        Unit unit = unitTransform.GetComponent<Unit>();
        UnitStats stats = unit.stats;

        DisplayTraits(_traitTextures, _traitLabels, stats.traits);

        _star.style.backgroundImage = Resources.Load<Texture2D>($"{GetStarImageName(unit.GetStar())}");
        _unitImage.style.backgroundImage = Resources.Load<Texture2D>($"{stats.type.ToString()}");
        _name.text = stats.type.ToString();
        _name.style.backgroundColor = _costColors[(int)stats.cost];

        float shield = unit.GetShield();
        float maxHealth = unit.GetMaxHealth() + shield;

        float shieldRatio = (unit.GetHealth() + shield) / maxHealth;
        float shieldPercent = Mathf.Lerp(0, 100, shieldRatio);
        _shieldBarMask.style.width = Length.Percent(shieldPercent);

        _healthLabel.text = $"{Mathf.Round(unit.GetHealth())}/{Mathf.Round(unit.GetMaxHealth())}";
        float healthRatio = unit.GetHealth() / maxHealth;
        float healthPercent = Mathf.Lerp(0, 100, healthRatio);
        _healthBarMask.style.width = Length.Percent(healthPercent);

        _manaLabel.text = $"{Mathf.Round(unit.GetMana())}/{stats.mana[1]}";
        float manaRatio = unit.GetMana() / stats.mana[1];
        float manaPercent = Mathf.Lerp(0, 100, manaRatio);
        _manaBarMask.style.width = Length.Percent(manaPercent);

        _ap.text = $"{unit.GetAP()}%";
        _ad.text = $"{unit.GetAD()}%";
        _baseDamage.text = $"{stats.attackDamage[(int)unit.GetStar()]}";
        _armor.text = $"{unit.GetArmor()}";
        _mr.text = $"{unit.GetMR()}";
        _atkSpeed.text = $"{unit.GetAS()}";
        _crit.text = $"{unit.GetCritChance()}%";
        _range.text = $"{unit.GetRange()}";
        _dr.text = $"{unit.GetDurability() * 100.0f}%";

        // _unitArt.material = unit.GetComponent<Renderer>().material;
        _unitDisplayBackground.visible = true;
    }

    public void HideUnitDisplay()
    {
        UIUtil.HideVisualElements(_traitTextures);
        _unitDisplayBackground.visible = false;
    }
}
