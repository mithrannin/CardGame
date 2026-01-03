using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ManaUI : MonoBehaviour
{
    [SerializeField] private PlayerMana playerMana;

    [System.Serializable]
    public struct ManaTextEntry
    {
        public CardSO.ManaColor manaColor;
        public TMP_Text manaText;
    }

    [SerializeField] private ManaTextEntry[] manaTextEntries;

    private Dictionary<CardSO.ManaColor, TMP_Text> lookup;

    private void Awake()
    {
        lookup = new Dictionary<CardSO.ManaColor, TMP_Text>();

        foreach (var entry in manaTextEntries)
        {
            lookup[entry.manaColor] = entry.manaText;
        }
        playerMana.OnManaChanged += UpdateManaUI;
        UpdateManaUI();
    }

    private void UpdateManaUI()
    {
        foreach (var pair in lookup)
        {
            int amount = playerMana.GetMana(pair.Key);
            pair.Value.text = amount.ToString();
        }

    }
}
