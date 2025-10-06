using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField] UIDocument UIDoc;
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

    private void InitTraits(ref VisualElement[] traitTextures, ref Label[] traitLabels)
    {
        traitTextures = new VisualElement[3];
        traitLabels = new Label[3];
        for (int i = 0; i < traitTextures.Length; i++)
        {
            int traitIndex = i + 1;
            traitTextures[i] = UIDoc.rootVisualElement.Q<VisualElement>($"Trait{traitIndex}");
            traitLabels[i] = UIDoc.rootVisualElement.Q<Label>($"Trait{traitIndex}");
        }
    }

    public void Init()
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

    // Update is called once per frame
    void Update()
    {
    }

    private void DisplayTraits(ref VisualElement[] visualElements, ref Label[] labels, Trait[] traits)
    {
        for (int i = 0; i < visualElements.Length; i++)
        {
            if (i < traits.Length
                && traits[i] != Trait.None)
            {
                Texture2D tex = Resources.Load<Texture2D>(UnitTrait.ToTexture(traits[i]));
                visualElements[i].style.backgroundImage = tex;
                labels[i].text = UnitTrait.ToString(traits[i]);
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

        DisplayTraits(ref _traitTextures, ref _traitLabels, stats.traits);

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
        Array.ForEach(_traitTextures, (VisualElement ve) => { ve.visible = false; });
        _unitDisplayBackground.visible = false;
    }
}
