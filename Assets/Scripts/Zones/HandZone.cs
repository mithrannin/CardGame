using System;
using System.Collections.Generic;
using UnityEngine;

public class HandZone : MonoBehaviour
{
    [SerializeField] private int maxHandSize = 12;

    private readonly List<Card> cards = new();

    [SerializeField] private HandLayout handLayout;

    public event Action OnHandChanged;

    public bool CanAddCard(Card card) => cards.Count < maxHandSize;

    public void AddCard(Card card)
    {
        if (!CanAddCard(card))
            return;

        cards.Add(card);
        card.SetLocation(CardLocation.Hand);
        card.transform.SetParent(handLayout.transform);
        handLayout.UpdateLayout(cards);
        OnHandChanged?.Invoke();
    }

    public void RemoveCard(Card card)
    {
        if (!cards.Contains(card))
            return;

        cards.Remove(card);
     
        card.transform.SetParent(null);
        handLayout.UpdateLayout(cards);      
        OnHandChanged?.Invoke();
    }

    public IReadOnlyList<Card> Cards => cards.AsReadOnly();

    public void RestoreCardPosition(Card card)
    {
        handLayout.UpdateLayout(cards);
    }

}
