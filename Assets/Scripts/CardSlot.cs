using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour
{
    public int slotIndex;
    public FieldZone FieldZone { get; private set; }
    public Card CardInSlot { get; private set; }
    public bool IsEmpty => CardInSlot == null;

    [SerializeField] private GameObject highlight;

    public void Initialize(FieldZone zone, int index)
    {
        FieldZone = zone;
        slotIndex = index;
    }

    public void SetHighlight(bool enabled)
    {
        highlight.SetActive(enabled);
    }

    public void SetCard(Card card)
    {
        CardInSlot = card;
        card.transform.SetParent(transform);
        card.MoveToPoint(transform.position, Quaternion.identity);

        if (card is CardUnit cardUnit)
        {
            cardUnit.SetCurrentSlot(this);
        }
    }

    public void ClearSlot()
    {
        if (IsEmpty)
            return;

        CardInSlot = null;
    }
}
