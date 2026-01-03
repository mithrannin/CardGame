using System.Collections.Generic;
using UnityEngine;

public class PlayerActionController : MonoBehaviour
{
    [SerializeField] private TurnSystemManager turnSystem;

    public bool TryReturnGraveyardToHand(PlayerContext player)
    {
        if (!turnSystem.CanTakeAction() || player != turnSystem.ActivePlayer)
            return false;

        var cards = player.Graveyard.RemoveAllCards();

        foreach (var card in cards)
        {
            player.Hand.AddCard(card);
            player.Controller.State.TakeDamage(1);
        }

        turnSystem.ConsumeAction();
        return true;
    }

    public bool TryGainMana(PlayerContext player)
    {
        if (!turnSystem.CanTakeAction() || player != turnSystem.ActivePlayer)
            return false;

        var color = player.Mana.GetRandomActiveColor();
        player.Mana.AddMana(color, 1);

        turnSystem.ConsumeAction();
        return true;
    }
}
