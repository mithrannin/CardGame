using UnityEngine;

public class PlayerContext
{
    public Deck Deck { get; }

    public PlayerController Controller { get; }
    public HandZone Hand {  get; }
    public FieldZone Field { get; }
    public GraveyardZone Graveyard { get; }
    public PlayerMana Mana { get; }
    public string Name { get; }

    public PlayerContext(string name, PlayerController controller, HandZone hand, FieldZone field, GraveyardZone graveyard, PlayerMana mana, Deck deck)
    {
        Name = name;
        Controller = controller;
        Hand = hand;
        Field = field;
        Graveyard = graveyard;
        Mana = mana;
        Deck = deck;
    }
}
