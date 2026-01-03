public static class PlayerActions
{
    public static bool TryReturnGraveyardToHand(Player player, TurnSystemManager turnSystem)
    {
        if (!turnSystem.CanTakeAction(player.Side))
            return false;

        var cards = player.Graveyard.RemoveAllCards();

        foreach (var card in cards)
        {
            player.Hand.AddCard(card);
            player.TakeDamage(1);
        }

        turnSystem.ConsumeAction();
        return true;
    }

    public static bool TryGainMana(Player player, TurnSystemManager turnSystem)
    {
        if (!turnSystem.CanTakeAction(player.Side))
            return false;

        var color = player.Mana.GetRandomActiveColor();
        player.Mana.AddMana(color, 1);

        turnSystem.ConsumeAction();
        return true;
    }
}
