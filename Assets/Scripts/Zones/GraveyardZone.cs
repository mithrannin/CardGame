using System;
using System.Collections.Generic;
using UnityEngine;

public class GraveyardZone : MonoBehaviour
{
    [SerializeField] private Transform topCardAnchor;

    private readonly List<Card> cards = new();

    public IReadOnlyList<Card> Cards => cards;

    public event Action OnChanged;

    public void AddCard(Card card)
    {
        cards.Add(card);
        card.SetLocation(CardLocation.Graveyard);
        card.transform.SetParent(transform);
        UpdateTopCardVisual();
        OnChanged?.Invoke();
    }

    public List<Card> RemoveAllCards()
    {
        List<Card> allCards = new(cards);
        cards.Clear();
        UpdateTopCardVisual();
        OnChanged?.Invoke();
        return allCards;
    }

    private void UpdateTopCardVisual()
    {
        if (cards.Count == 0)
        {
            return;
        }

        Card topCard = cards[^1];
        topCard.MoveToPoint(topCardAnchor.position, topCardAnchor.rotation);
    }
}
