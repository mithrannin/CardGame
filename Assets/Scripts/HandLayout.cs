using System.Collections.Generic;
using UnityEngine;

public class HandLayout : MonoBehaviour
{
    [SerializeField] private Transform minPosition;
    [SerializeField] private Transform maxPosition;
    [SerializeField] private float cardRotation = -6f;

    public void UpdateLayout(IReadOnlyList<Card> cards)
    {
        if (cards.Count == 0)
            return;

        Vector3 spacing = cards.Count > 1 ? (maxPosition.position - minPosition.position) / (cards.Count - 1) : Vector3.zero;

        for (int i = 0; i < cards.Count; i++)
        {
            Vector3 targetPosition = minPosition.position + spacing * i;
            cards[i].MoveToPoint(targetPosition, Quaternion.Euler(0, 0, cardRotation));
        }
    }
}
