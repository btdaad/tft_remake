using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

public class UIManager : MonoBehaviour
{
    [SerializeField] UIDocument UIDoc;

    #region UnitDisplay
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
    #endregion

    #region SynergyDisplay
    private int MAX_SYNERGIES_DISPLAYED = 6;
    private VisualElement[] _activeSynergies;
    private VisualElement[] _passiveSynergies;
    private Button _showMore;
    #endregion

    private void HideVisualElements(VisualElement[] visualElements)
    {
        Array.ForEach(visualElements, (VisualElement ve) => { ve.visible = false; });
    }

    private void InitTraits(ref VisualElement[] traitTextures, ref Label[] traitLabels)
    {
        traitTextures = new VisualElement[MAX_TRAITS_DISPLAYED];
        traitLabels = new Label[MAX_TRAITS_DISPLAYED];
        for (int i = 0; i < MAX_TRAITS_DISPLAYED; i++)
        {
            int traitIndex = i + 1;
            traitTextures[i] = UIDoc.rootVisualElement.Q<VisualElement>($"Trait{traitIndex}");
            traitLabels[i] = UIDoc.rootVisualElement.Q<Label>($"Trait{traitIndex}");
        }
    }

    private void InitUnitDisplay()
    {
        _unitDisplayBackground = UIDoc.rootVisualElement.Q<VisualElement>("UnitDisplayBackground");

        InitTraits(ref _traitTextures, ref _traitLabels);
        _name = UIDoc.rootVisualElement.Q<Label>("Name");

        _healthLabel = UIDoc.rootVisualElement.Q<Label>("HealthLabel");
        _healthBarMask = UIDoc.rootVisualElement.Q<VisualElement>("HealthBarMask");
        _manaLabel = UIDoc.rootVisualElement.Q<Label>("ManaLabel");
        _manaBarMask = UIDoc.rootVisualElement.Q<VisualElement>("ManaBarMask");

        _baseDamage = UIDoc.rootVisualElement.Q<Label>("BaseDamage");
        _ap = UIDoc.rootVisualElement.Q<Label>("AP");
        _ad = UIDoc.rootVisualElement.Q<Label>("AD");
        _armor = UIDoc.rootVisualElement.Q<Label>("Armor");
        _mr = UIDoc.rootVisualElement.Q<Label>("MR");
        _atkSpeed = UIDoc.rootVisualElement.Q<Label>("AtkSpeed");
        _crit = UIDoc.rootVisualElement.Q<Label>("Crit");

        _unitDisplayBackground.visible = false;
    }

    private void InitSynergyDisplay()
    {
        _activeSynergies = new VisualElement[MAX_SYNERGIES_DISPLAYED];
        _passiveSynergies = new VisualElement[MAX_SYNERGIES_DISPLAYED];
        for (int i = 0; i < MAX_SYNERGIES_DISPLAYED; i++)
        {
            int synergyIndex = i + 1;
            _activeSynergies[i] = UIDoc.rootVisualElement.Q<VisualElement>($"ActiveSynergy{synergyIndex}");
            _passiveSynergies[i] = UIDoc.rootVisualElement.Q<VisualElement>($"PassiveSynergy{synergyIndex}");
        }
        HideVisualElements(_activeSynergies);
        HideVisualElements(_passiveSynergies);

        _showMore = UIDoc.rootVisualElement.Q<Button>("ShowMore");
        _showMore.visible = false;
    }

    public void Init()
    {
        InitUnitDisplay();
        InitSynergyDisplay();
    }

    // Update is called once per frame
    void Update()
    {
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

        _name.text = unit.name;

        _healthLabel.text = $"{unit.GetHealth()}/{stats.health[(int)stats.star]}";
        _manaLabel.text = $"{unit.GetMana()}/{stats.mana[1]}";

        _ap.text = $"{unit.GetAP()}%";
        _ad.text = $"{unit.GetAD()}%";
        _baseDamage.text = $"{stats.attackDamage[(int)stats.star]}";
        _armor.text = $"{stats.armor}";
        _mr.text = $"{stats.magicResist}";
        _atkSpeed.text = $"{stats.attackSpeed}";
        _crit.text = $"{stats.critChance}%";

        // _unitArt.material = unit.GetComponent<Renderer>().material;
        _unitDisplayBackground.visible = true;
    }

    public void HideUnitDisplay()
    {
        HideVisualElements(_traitTextures);
        _unitDisplayBackground.visible = false;
    }

    public void UpdateSynergyDisplay(Dictionary<Trait, List<Transform>> unsortedSynergies, UnitTraitSO[] unitTraits)
    {
        var synergies = from entry in unsortedSynergies orderby entry.Value.Count descending select entry;
        synergies.ToDictionary(pair => pair.Key, pair => pair.Value);

        foreach (KeyValuePair<Trait, List<Transform>> kvp in synergies)
        {
            Trait trait = kvp.Key;
            int nbUnit = kvp.Value.Count();
            // TODO: check they are different unit !;
            UnitTraitSO traitSO = Array.Find(unitTraits, (UnitTraitSO unitTrait) => { return unitTrait.trait == trait; });
        }
    }
}
