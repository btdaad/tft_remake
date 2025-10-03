using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class UIManager : MonoBehaviour
{
    [SerializeField] UIDocument UIDoc;
    private VisualElement _unitDisplayBackground;
    private Label _healthLabel;
    private VisualElement _healthBarMask;
    private Label _manaLabel;
    private VisualElement _manaBarMask;
    private Label _name;
    private Label _ap;
    private Label _ad;
    private Label _armor;
    private Label _rm;
    private Label _atkSpeed;
    private Label _crit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Init()
    {
        _unitDisplayBackground = UIDoc.rootVisualElement.Q<VisualElement>("UnitDisplayBackground");
        _healthLabel = UIDoc.rootVisualElement.Q<Label>("HealthLabel");
        _healthBarMask = UIDoc.rootVisualElement.Q<VisualElement>("HealthBarMask");
        _manaLabel = UIDoc.rootVisualElement.Q<Label>("ManaLabel");
        _manaBarMask = UIDoc.rootVisualElement.Q<VisualElement>("ManaBarMask");
        _name = UIDoc.rootVisualElement.Q<Label>("Name");
        _ap = UIDoc.rootVisualElement.Q<Label>("AP");
        _ad = UIDoc.rootVisualElement.Q<Label>("AD");
        _armor = UIDoc.rootVisualElement.Q<Label>("Armor");
        _rm = UIDoc.rootVisualElement.Q<Label>("RM");
        _atkSpeed = UIDoc.rootVisualElement.Q<Label>("AtkSpeed");
        _crit = UIDoc.rootVisualElement.Q<Label>("Crit");

        _unitDisplayBackground.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ShowUnitDisplay(Transform unitTransform)
    {
        Unit unit = unitTransform.GetComponent<Unit>(); 
        UnitStats stats = unit.stats;

        _healthLabel.text = $"{unit.GetHealth()}/{stats.health[(int) stats.star]}";
        _manaLabel.text = $"{unit.GetMana()}/{stats.mana[(int) stats.star]}";

        _name.text = unit.name;

        _ap.text = $"{stats.abilityPower}";
        _ad.text = $"{stats.attackDamage[(int)stats.star]}";
        _armor.text = $"{stats.armor}";
        _rm.text = $"{stats.magicResist}";
        _atkSpeed.text = $"{stats.attackSpeed}";
        _crit.text = $"{stats.critChance}%";

        // _unitArt.material = unit.GetComponent<Renderer>().material;
        _unitDisplayBackground.visible = true;
    }

    public void HideUnitDisplay()
    {
        _unitDisplayBackground.visible = false;
    }
}
