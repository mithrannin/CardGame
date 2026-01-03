using System.Collections;
using UnityEngine;

public class AIController : MonoBehaviour
{
    private Player aiPlayer;
    private Player opponent;

    private TurnSystemManager turnSystem;

    public void Initialize(Player ai, Player human, TurnSystemManager turnSystemManager)
    {
        aiPlayer = ai;
        opponent = human;
        turnSystem = turnSystemManager;
    }

    private bool TryPlayBestCard()
    {
        Card bestCard = null;
        CardSlot bestSlot = null;
        float bestScore = float.MinValue;

        foreach (Card card in aiPlayer.Hand.Cards)
        {
            if (!aiPlayer.Mana.CanPayCost(card))
                continue;

            if (card is CardUnit unit)
            {
                (CardSlot slot, float score) = EvaluateUnitPlacement(unit);

                if (score > bestScore)
                {
                    bestCard = card;
                    bestSlot = slot;
                    bestScore = score;
                }
            }
        }

        if (bestCard != null && bestSlot != null)
        {
            PlayCardOnSlot(bestCard as CardUnit, bestSlot);
            return true;
        }

        return false;
    }

    private (CardSlot slot, float score) EvaluateUnitPlacement(CardUnit unit)
    {
        CardSlot bestSlot = null;
        float bestScore = float.MinValue;

        foreach (CardSlot slot in aiPlayer.Field.Slots)
        {
            if (!slot.IsEmpty)
                continue;

            float score = ScoreSlotPlacement(slot, unit);

            if (score > bestScore)
            {
                bestScore = score;
                bestSlot = slot;
            }
        }

        return (bestSlot, bestScore);
    }

    private float ScoreSlotPlacement(CardSlot mySlot, CardUnit unit)
    {
        float score = 0f;

        CardSlot opposingSlot = opponent.Field.GetOpposingSlot(mySlot);

        if (opposingSlot != null && !opposingSlot.IsEmpty)
        {
            if (opposingSlot.CardInSlot is CardUnit enemyUnit)
            {
                score += 10f;
                if (unit.Attack >= enemyUnit.Health)
                    score += 15f;
                if (unit.Health <= enemyUnit.Attack)
                    score -= 20f;
            }
        }
        else
        {
            score += 5f;
        }

        return score;
    }

    private bool TryDiscardForMana()
    {
        Card worstCard = null;
        float worstScore = float.MaxValue;

        foreach (Card card in aiPlayer.Hand.Cards)
        {
            if (aiPlayer.Mana.CanPayCost(card))
                continue;

            float manaDifference = card.CurrentManaCost - aiPlayer.Mana.GetMana(card.CardData.manaColor);

            if (manaDifference == 1)
            {
                worstCard = card;
                break;
            }

            float score = EvaluateCardValue(card);
            if (score < worstScore)
            {
                worstScore = score;
                worstCard = card;
            }
        }

        if (worstCard != null)
        {
            DiscardCard(worstCard);
            return true;
        }

        return false;
    }

    private float EvaluateCardValue(Card card)
    {
        if (card is CardUnit unit)
            return (unit.Attack + unit.Health) / (float)card.CurrentManaCost;

        return 1f;
    }

    private bool ShouldResurrectGraveyard()
    {
        if (aiPlayer.Graveyard.Cards.Count < 3)
            return false;

        int graveyardValue = 0;
        foreach (Card card in aiPlayer.Graveyard.Cards)
        {
            if (card is CardUnit unit)
                graveyardValue += unit.Attack + unit.Health;
        }

        int healthCost = aiPlayer.Graveyard.Cards.Count;

        return graveyardValue > healthCost * 5 && aiPlayer.Life > healthCost + 5;
    }

    public IEnumerator TakeTurn()
    {
        while (turnSystem.CanTakeAction(PlayerSide.AI))
        {
            yield return new WaitForSeconds(0.5f);

            if (ShouldResurrectGraveyard())
            {
                PlayerActions.TryReturnGraveyardToHand(aiPlayer, turnSystem);
                turnSystem.ConsumeAction();
                continue;
            }

            if (TryPlayBestCard())
            {
                turnSystem.ConsumeAction();
                continue;
            }

            if (TryDiscardForMana())
            {
                turnSystem.ConsumeAction();
                continue;
            }

            PlayerActions.TryGainMana(aiPlayer, turnSystem);
            turnSystem.ConsumeAction();
        }
    }

    private void PlayCardOnSlot(CardUnit unit, CardSlot slot)
    {
        aiPlayer.Mana.PayCost(unit);
        aiPlayer.Hand.RemoveCard(unit);
        aiPlayer.Field.PlaceCard(unit, slot);
        GameplayController.Instance.OnUnitPlayed(unit);
    }

    private void DiscardCard(Card card)
    {
        aiPlayer.Hand.RemoveCard(card);
        aiPlayer.Graveyard.AddCard(card);
        aiPlayer.Mana.AddMana(card.CardData.manaColor, 1);
    }
}
