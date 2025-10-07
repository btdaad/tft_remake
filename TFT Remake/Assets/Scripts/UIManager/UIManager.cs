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
    private int MAX_STAGES = 4;
    private Color GRAY = new Color(106.0f / 255.0f, 102.0f / 255.0f, 102.0f / 255.0f);
    private Color WHITE = new Color(255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f);
    private VisualElement[] _activeSynergies;
    private VisualElement[] _passiveSynergies;
    private Button _showMore;
    #endregion

    private T GetUIElement<T>(string name) where T : UnityEngine.UIElements.VisualElement
    {
        return UIDoc.rootVisualElement.Q<T>(name);
    }

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

    public void HideActiveSynergyStages()
    {
        for (int i = 0; i < _activeSynergies.Length; i++)
        {
            int synergyIndex = i + 1;
            for (int j = 0; j < MAX_STAGES; j++)
            {
                int stageIndex = j + 1;
                Label curStage = UIDoc.rootVisualElement.Q<Label>($"{stageIndex}Stage{synergyIndex}");
                curStage.visible = false;
                Label curSeparator = UIDoc.rootVisualElement.Q<Label>($"{stageIndex}Separator{synergyIndex}");
                if (stageIndex != MAX_STAGES) // there is one separator less
                    curSeparator.visible = true;
            }
        }
    }

    // returns -1 if the synergy is passive
    // otherwise, returns the index of the current stage
    private int GetCurrentStageIndex(int[] stages, int nbUnit)
    {
        int currentStageIndex = -1;
        for (int i = 0; i < stages.Length; i++)
        {
            if (stages[i] <= nbUnit)
                currentStageIndex = i;
        }
        return currentStageIndex;
    }

    private Dictionary<Trait, List<Transform>> RemoveIdenticalUnits(Dictionary<Trait, List<Transform>> synergies)
    {
        Dictionary<Trait, List<Transform>> dict = new Dictionary<Trait, List<Transform>>();
        foreach (KeyValuePair<Trait, List<Transform>> kvp in synergies)
        {
            Trait trait = kvp.Key;
            List<Transform> units = new List<Transform>();
            foreach (Transform transform in kvp.Value)
            {
                if (!units.Exists(unit => unit.gameObject.GetComponent<Unit>().stats.type == transform.gameObject.GetComponent<Unit>().stats.type))
                    units.Add(transform); 
            }
            dict[trait] = units;
        }
        return dict;
    }

    public void UpdateSynergyDisplay(Dictionary<Trait, List<Transform>> unsortedSynergies, UnitTraitSO[] unitTraits)
    {
        HideVisualElements(_activeSynergies);
        HideVisualElements(_passiveSynergies);
        HideActiveSynergyStages();

        unsortedSynergies = RemoveIdenticalUnits(unsortedSynergies);
        Dictionary<Trait, List<Transform>> synergies = unsortedSynergies
                        .OrderByDescending(kvp => kvp.Value.Count >= Array.Find(unitTraits, (UnitTraitSO unitTrait) => { return unitTrait.trait == kvp.Key; }).stages[0]) // Active synergies appear before Passive
                        .ThenByDescending(kvp => kvp.Value.Count) // the more unit there is the higher it appears
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value); // converts IEnumerable to dictionary

        int nbActiveSynergy = 0;
        int nbPassiveSynergy = 0;
        foreach (KeyValuePair<Trait, List<Transform>> kvp in synergies)
        {
            if (nbActiveSynergy + nbPassiveSynergy == MAX_SYNERGIES_DISPLAYED)
                break;
            int synergyIndex = nbActiveSynergy + nbPassiveSynergy + 1;

            Trait trait = kvp.Key;
            int nbUnit = kvp.Value.Count();
            UnitTraitSO traitSO = Array.Find(unitTraits, (UnitTraitSO unitTrait) => { return unitTrait.trait == trait; });

            int currentStageIndex = GetCurrentStageIndex(traitSO.stages, nbUnit);
            if (currentStageIndex != -1) // Active Synergy
            {
                Label nbActiveUnit = UIDoc.rootVisualElement.Q<Label>($"LevelLabel{synergyIndex}");
                nbActiveUnit.text = nbUnit.ToString();

                VisualElement traitIcon = UIDoc.rootVisualElement.Q<VisualElement>($"Symbol{synergyIndex}");
                Texture2D tex = Resources.Load<Texture2D>(TraitUtil.ToTexture(trait));
                traitIcon.style.backgroundImage = tex;

                Label traitName = UIDoc.rootVisualElement.Q<Label>($"ActiveTrait{synergyIndex}");
                traitName.text = TraitUtil.ToString(trait);

                int nbStages = traitSO.nbStages;
                for (int i = 0; i < MAX_STAGES; i++)
                {
                    int stageIndex = i + 1;
                    Label curStage = UIDoc.rootVisualElement.Q<Label>($"{stageIndex}Stage{synergyIndex}");
                    Label curSeparator = UIDoc.rootVisualElement.Q<Label>($"{stageIndex}Separator{synergyIndex}");
                    if (i < nbStages) // display
                    {
                        curStage.text = traitSO.stages[i].ToString();
                        curStage.visible = true;

                        if (i == currentStageIndex)
                            curStage.style.color = WHITE;
                        else
                            curStage.style.color = GRAY;

                        if (stageIndex != MAX_STAGES) // there is one separator less
                        {
                            if (stageIndex < nbStages) // no need to display a separator if it's the last stage
                                curSeparator.visible = true;
                            else
                                curSeparator.visible = false;
                        }
                    }
                    else // hide
                    {
                        curStage.visible = false;
                        if (stageIndex != MAX_STAGES)
                            curSeparator.visible = false;
                    }
                }

                _activeSynergies[nbActiveSynergy + nbPassiveSynergy].visible = true;
                nbActiveSynergy++;
            }
            else // Passive Synergy
            {
                VisualElement traitIcon = UIDoc.rootVisualElement.Q<VisualElement>($"PassiveSymbol{synergyIndex}");
                Texture2D tex = Resources.Load<Texture2D>(TraitUtil.ToTexture(trait));
                traitIcon.style.backgroundImage = tex;

                Label traitName = UIDoc.rootVisualElement.Q<Label>($"PassiveTrait{synergyIndex}");
                traitName.text = TraitUtil.ToString(trait);

                Label actualNb = UIDoc.rootVisualElement.Q<Label>($"curStage{synergyIndex}");
                actualNb.text = nbUnit.ToString();

                Label expectedNb = UIDoc.rootVisualElement.Q<Label>($"nextStage{synergyIndex}");
                expectedNb.text = traitSO.stages[0].ToString();

                _passiveSynergies[nbActiveSynergy + nbPassiveSynergy].visible = true;
                nbPassiveSynergy++;
            }
        }
    }
}
