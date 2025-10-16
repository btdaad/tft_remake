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
    private Button _fight;
    private Button _changePlayer;
    private Label _gold;

    public void Init()
    {
        UIUnit = UnitsDisplay.Instance(UIDoc);
        UIUnit.InitUnitDisplay();
        UISynergy = SynergiesDisplay.Instance(UIDoc);
        UISynergy.InitSynergyDisplay();

        _fight = UIDoc.rootVisualElement.Q<Button>("Fight");
        _fight.clickable.clicked += () => { GameManager.Instance.Fight(); };
        _changePlayer = UIDoc.rootVisualElement.Q<Button>("ChangePlayer");
        _changePlayer.clickable.clicked += () => { GameManager.Instance.ChangePlayer(); };
        _gold = UIDoc.rootVisualElement.Q<Label>("Gold");
        Button dumpButton = UIDoc.rootVisualElement.Q<Button>("Dump");
        dumpButton.clickable.clicked += () => { GameManager.Instance.Dump(); };
    }

    public void UpdateSynergyDisplay(Dictionary<Trait, List<Transform>> unsortedSynergies, UnitTraitSO[] unitTraits)
    {
        UISynergy.UpdateSynergyDisplay(unsortedSynergies, unitTraits);
    }

    public void UpdateGold(bool isPlayer)
    {
        _gold.text = $"{GameManager.Instance.GetGoldManager().GetGold(isPlayer)}";
    }

    public void ShowUnitDisplay(Transform unitTransform)
    {
        UIUnit.ShowUnitDisplay(unitTransform);
    }

    public void HideUnitDisplay()
    {
        UIUnit.HideUnitDisplay();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ChangePlayer(bool isPlayer)
    {
        if (isPlayer)
            _changePlayer.text = "Play as opponent";
        else
            _changePlayer.text = "Play as yourself";
        UpdateGold(isPlayer);
    }
}
