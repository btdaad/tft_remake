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
    private VisualElement[] _traitTextures;
    private Label[] _traitLabels;
    private Label _name;
    private Label _healthLabel;
    private VisualElement _healthBarMask;
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

        InitTraits(ref _traitTextures, ref _traitLabels);
        _name = GetUIElement<Label>("Name");

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

    public void ShowUnitDisplay(Transform unitTransform)
    {
        Unit unit = unitTransform.GetComponent<Unit>();
        UnitStats stats = unit.stats;

        DisplayTraits(_traitTextures, _traitLabels, stats.traits);

        _name.text = stats.type.ToString();

        _healthLabel.text = $"{unit.GetHealth()}/{stats.health[(int)stats.star]}";
        _manaLabel.text = $"{unit.GetMana()}/{stats.mana[1]}";

        _ap.text = $"{unit.GetAP()}%";
        _ad.text = $"{unit.GetAD()}%";
        _baseDamage.text = $"{stats.attackDamage[(int)stats.star]}";
        _armor.text = $"{stats.armor}";
        _mr.text = $"{stats.magicResist}";
        _atkSpeed.text = $"{stats.attackSpeed}";
        _crit.text = $"{stats.critChance}%";
        _range.text = $"{stats.range}";

        // _unitArt.material = unit.GetComponent<Renderer>().material;
        _unitDisplayBackground.visible = true;
    }

    public void HideUnitDisplay()
    {
        UIUtil.HideVisualElements(_traitTextures);
        _unitDisplayBackground.visible = false;
    }
}
