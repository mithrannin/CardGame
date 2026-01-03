using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Deck
{
    [SerializeField] private List<CardSO> cards = new();

    public IReadOnlyList<CardSO> Cards => cards;

    public void Validate()
    {
        if (cards.Count != 12)
            Debug.LogWarning($"Deck has {cards.Count} cards (expected 12)");
    }

    public IEnumerable<CardSO> GetAllCards()
    {
        foreach (var card in cards)
            yield return card;
    }

    public HashSet<CardSO.ManaColor> GetUsedManaColors()
    {
        HashSet<CardSO.ManaColor> colors = new();
        foreach (var card in cards)
            colors.Add(card.manaColor);

        return colors;
    }
}
