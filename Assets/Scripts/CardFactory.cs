using UnityEngine;

public class CardFactory : MonoBehaviour
{
    [SerializeField] private Card unitPrefab;
    [SerializeField] private Card spellPrefab;

    public Card Create(CardSO data)
    {
        Card card = data switch
        {
            CardUnitSO => Instantiate(unitPrefab),
            //CardSpellSO => Instantiate(spellPrefab),
            _ => throw new System.Exception("Unknown card type")
        };

        card.InitializeCard();
        return card;
    }
}
