using UnityEngine;

public class CardPlayResolver : MonoBehaviour
{
    public bool TryResolveDrop(Card card, Player player, Player opponent)
    {
        int layerMask = ~LayerMask.GetMask("Card");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            return false;
        }

        if (TryResolveGraveyardDrop(card, hit, player))
            return true;

        if (!player.Mana.CanPayCost(card))
        {
            return false;
        }

        if (card is CardSpell spell)
            return TryResolveSpell(spell, hit, player, opponent);

        else if (card is CardUnit unit)
            return TryResolveUnit(unit, hit, player);

        return false;
    }

    private bool TryResolveSpell(CardSpell spell, RaycastHit hit, Player player, Player opponent)
    {
        if (!spell.CanResolveOn(hit, player, opponent))
            return false;

        player.Mana.PayCost(spell);
        player.Hand.RemoveCard(spell);

        spell.ResolveSpell(player, opponent, hit);

        return true;
    }

    private bool TryResolveUnit(CardUnit unit, RaycastHit hit, Player player)
    {
        CardSlot slot = hit.collider.GetComponent<CardSlot>();
        if (slot == null || !slot.IsEmpty)
        {
            return false;
        }

        player.Mana.PayCost(unit);
        player.Hand.RemoveCard(unit);
        player.Field.PlaceCard(unit, slot);
        GameplayController.Instance.OnUnitPlayed(unit);

        return true;
    }

    private bool TryResolveGraveyardDrop(Card card, RaycastHit hit, Player player)
    {
        GraveyardZone graveyard = hit.collider.GetComponent<GraveyardZone>();
        if (graveyard == null)
        {
            return false;
        }

        player.Hand.RemoveCard(card);
        player.Graveyard.AddCard(card);
        player.Mana.AddMana(card.CardData.manaColor, 1);

        return true;
    }
}

