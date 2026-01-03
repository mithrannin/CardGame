using UnityEngine;

public class CardPlayResolver : MonoBehaviour
{
    public bool TryResolveDrop(Card card, PlayerContext player, PlayerContext opponent)
    {
        if (!player.Mana.CanPayCost(card))
        {
            return false;
        }

        int layerMask = ~LayerMask.GetMask("Card");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            return false;
        }

        bool resolved = false;
        if (card is CardSpell spell)
            resolved = TryResolveSpell(spell, hit, player, opponent);

        else if (card is CardUnit unit)
            resolved = TryResolveUnit(unit, hit, player);

        if (!resolved)
            resolved = TryResolveGraveyardDrop(card, hit, player);

        return resolved;
    }

    private bool TryResolveSpell(CardSpell spell, RaycastHit hit, PlayerContext player, PlayerContext opponent)
    {
        if (!spell.CanResolveOn(hit, player, opponent))
            return false;

        player.Mana.PayCost(spell);
        player.Hand.RemoveCard(spell);

        spell.ResolveSpell(player, opponent, hit);

        return true;
    }

    private bool TryResolveUnit(CardUnit unit, RaycastHit hit, PlayerContext player)
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

    private bool TryResolveGraveyardDrop(Card card, RaycastHit hit, PlayerContext player)
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

