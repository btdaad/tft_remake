using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;

public class SynergiesDisplay
{
    private static SynergiesDisplay _instance;
    private static UIDocument _uiDoc;

    public static SynergiesDisplay Instance(UIDocument uiDoc)
    { 
        if (_instance == null)
        {
            _instance = new SynergiesDisplay();
            _uiDoc = uiDoc;
        }
        return _instance;
    }

    private int MAX_SYNERGIES_DISPLAYED = 6;
    private int MAX_STAGES = 4;
    private Color GRAY = new Color(106.0f / 255.0f, 102.0f / 255.0f, 102.0f / 255.0f);
    private Color WHITE = new Color(255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f);
    private VisualElement[] _activeSynergies;
    private VisualElement[] _passiveSynergies;
    private Button _showMore;
    private bool _isShowMoreClicked = false;
    
    private T GetUIElement<T>(string name) where T : UnityEngine.UIElements.VisualElement
    {
        return _uiDoc.rootVisualElement.Q<T>(name);
    }

    public void InitSynergyDisplay()
    {
        _activeSynergies = new VisualElement[MAX_SYNERGIES_DISPLAYED];
        _passiveSynergies = new VisualElement[MAX_SYNERGIES_DISPLAYED];
        for (int i = 0; i < MAX_SYNERGIES_DISPLAYED; i++)
        {
            int synergyIndex = i + 1;
            _activeSynergies[i] = GetUIElement<VisualElement>($"ActiveSynergy{synergyIndex}");
            _passiveSynergies[i] = GetUIElement<VisualElement>($"PassiveSynergy{synergyIndex}");
        }
        UIUtil.HideVisualElements(_activeSynergies);
        UIUtil.HideVisualElements(_passiveSynergies);

        _showMore = GetUIElement<Button>("ShowMore");
        _showMore.visible = false;
        _showMore.clickable.clicked += () => { _isShowMoreClicked = !_isShowMoreClicked; };
        // _showMore.clickable.activators.Clear();
        // _showMore.RegisterCallback<MouseDownEvent>(e => { _isShowMoreClicked = !_isShowMoreClicked; });
    }

    // as the text has been specifically set to visible or has been hidden to feat the number of stages of each Trait
    // it is necessary to hide them manually
    public void HideActiveSynergyStages()
    {
        for (int i = 0; i < _activeSynergies.Length; i++)
        {
            int synergyIndex = i + 1;
            for (int j = 0; j < MAX_STAGES; j++)
            {
                int stageIndex = j + 1;
                Label curStage = GetUIElement<Label>($"{stageIndex}Stage{synergyIndex}");
                curStage.visible = false;
                Label curSeparator = GetUIElement<Label>($"{stageIndex}Separator{synergyIndex}");
                if (stageIndex != MAX_STAGES) // there is one separator less
                    curSeparator.visible = true;
            }
        }
    }

    // Used to know at which stage of the synergy the player is, depending on the number of distinct unit of the same Trait on the board
    // Returns -1 if the synergy is passive; otherwise, returns the index of the current stage
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

    // When computing the number of unit for which a Trait applies, it is necessary to remove similar units as they should only count for 1
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

    /*
     * @param synergyIndex : Index of the current synergy (1 to 6) in UI indexing (starts at 1)
     * @param nbUnit : Current number of distinct unit of the trait on the board
     * @param trait : Trait of the synergy
     * @param traitSO : Holds information about the number of unit necessary to validate a stage
     * @param currentStageIndex : Index of the current stage in the synergy (based on traitSO.stages)
     */
    private void DisplaActiveSynergy(int synergyIndex, int nbUnit, Trait trait, UnitTraitSO traitSO, int currentStageIndex)
    {
        VisualElement traitIcon = GetUIElement<VisualElement>($"Symbol{synergyIndex}");
        Texture2D tex = Resources.Load<Texture2D>(TraitUtil.ToTexture(trait));
        traitIcon.style.backgroundImage = tex;

        Label traitName = GetUIElement<Label>($"ActiveTrait{synergyIndex}");
        traitName.text = TraitUtil.ToString(trait);

        Label actualNb = GetUIElement<Label>($"LevelLabel{synergyIndex}");
        actualNb.text = nbUnit.ToString();

        int nbStages = traitSO.nbStages;
        for (int i = 0; i < MAX_STAGES; i++)
        {
            int stageIndex = i + 1;
            Label curStage = GetUIElement<Label>($"{stageIndex}Stage{synergyIndex}");
            Label curSeparator = GetUIElement<Label>($"{stageIndex}Separator{synergyIndex}");
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
    }

    /*
     * @param synergyIndex : Index of the current synergy (1 to 6) in UI indexing (starts at 1)
     * @param nbUnit : Current number of distinct unit of the trait on the board
     * @param trait : Trait of the synergy
     * @param traitSO : Holds information about the number of unit necessary to validate a stage
     */
    private void DisplayPassiveSynergy(int synergyIndex, int nbUnit, Trait trait, UnitTraitSO traitSO)
    {
        VisualElement traitIcon = GetUIElement<VisualElement>($"PassiveSymbol{synergyIndex}");
        Texture2D tex = Resources.Load<Texture2D>(TraitUtil.ToTexture(trait));
        traitIcon.style.backgroundImage = tex;

        Label traitName = GetUIElement<Label>($"PassiveTrait{synergyIndex}");
        traitName.text = TraitUtil.ToString(trait);

        Label actualNb = GetUIElement<Label>($"curStage{synergyIndex}");
        actualNb.text = nbUnit.ToString();

        Label expectedNb = GetUIElement<Label>($"nextStage{synergyIndex}");
        expectedNb.text = traitSO.stages[0].ToString();
    }

    public void UpdateSynergyDisplay(Dictionary<Trait, List<Transform>> unsortedSynergies, UnitTraitSO[] unitTraits)
    {
        UIUtil.HideVisualElements(_activeSynergies);
        UIUtil.HideVisualElements(_passiveSynergies);
        HideActiveSynergyStages();
        if (unsortedSynergies.Count <= MAX_SYNERGIES_DISPLAYED)
        {
            _showMore.visible = false;
            _isShowMoreClicked = false;
        }
        else
            _showMore.visible = true;

        unsortedSynergies = RemoveIdenticalUnits(unsortedSynergies);
        Dictionary<Trait, List<Transform>> synergies = unsortedSynergies
                        .OrderByDescending(kvp => kvp.Value.Count >= Array.Find(unitTraits, (UnitTraitSO unitTrait) => { return unitTrait.trait == kvp.Key; }).stages[0]) // Active synergies appear before Passive
                        .ThenByDescending(kvp => kvp.Value.Count) // the more unit there is the higher it appears
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value); // converts IEnumerable to dictionary

        int nbActiveSynergy = 0;
        int nbPassiveSynergy = 0;
        int skipShowMore = 0;
        foreach (KeyValuePair<Trait, List<Transform>> kvp in synergies)
        {
            if (_isShowMoreClicked && skipShowMore <= MAX_SYNERGIES_DISPLAYED)
            {
                skipShowMore++;
                continue;
            }

            if (nbActiveSynergy + nbPassiveSynergy == MAX_SYNERGIES_DISPLAYED) // cannot display more than MAX_SYNERGIES_DISPLAYED
                break;
            int synergyIndex = nbActiveSynergy + nbPassiveSynergy + 1; // used to interact with the UI elements (index starts at 1)

            Trait trait = kvp.Key;
            int nbUnit = kvp.Value.Count();
            UnitTraitSO traitSO = Array.Find(unitTraits, (UnitTraitSO unitTrait) => { return unitTrait.trait == trait; });

            int currentStageIndex = GetCurrentStageIndex(traitSO.stages, nbUnit);
            if (currentStageIndex != -1) // Active Synergy
            {
                DisplaActiveSynergy(synergyIndex, nbUnit, trait, traitSO, currentStageIndex);
                _activeSynergies[nbActiveSynergy + nbPassiveSynergy].visible = true;
                nbActiveSynergy++;
            }
            else // Passive Synergy
            {
                DisplayPassiveSynergy(synergyIndex, nbUnit, trait, traitSO);
                _passiveSynergies[nbActiveSynergy + nbPassiveSynergy].visible = true;
                nbPassiveSynergy++;
            }
        }
    }
}
