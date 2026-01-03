using UnityEngine;
using System.Collections.Generic;

public class FieldZone : MonoBehaviour
{
    [SerializeField] private CardSlot[] cardSlots;

    private void Awake()
    {
        for (int i = 0; i < cardSlots.Length; i++)
        {
            cardSlots[i].Initialize(this, i);
        }
    }

    public void PlaceCard(Card card, CardSlot slot)
    {
        slot.SetCard(card);
        card.SetLocation(CardLocation.Field);
    }

    public IEnumerable<CardSlot> Slots => cardSlots;

    public IEnumerable<CardUnit> Units
    {
        get
        {
            foreach(var slot in cardSlots)
            {
                if (slot.CardInSlot is CardUnit unit)
                    yield return unit;
            }

        }
    }

    public CardSlot GetOpposingSlot(CardSlot slot)
    {
        if (slot == null) 
            return null;

        int index = slot.slotIndex;
        if (index < 0 || index >= cardSlots.Length)
            return null;

        return cardSlots[index];
    }
}
