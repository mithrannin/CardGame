using System;

public class Player
{
    public string Name { get; }
    public PlayerSide Side { get; }

    public int Life { get; private set; }
    public event Action OnLifeChanged;

    public Deck Deck { get; }
    public HandZone Hand { get; }
    public FieldZone Field { get; }
    public GraveyardZone Graveyard { get; }
    public PlayerMana Mana { get; }

    private const int STARTING_LIFE = 30;

    public Player(string name, PlayerSide side, HandZone hand, FieldZone field, GraveyardZone graveyard, PlayerMana mana, Deck deck)
    {
        Name = name;
        Side = side;
        Hand = hand;
        Field = field;
        Graveyard = graveyard;
        Mana = mana;
        Deck = deck;
        Life = STARTING_LIFE;
    }

    public void TakeDamage(int amount)
    {
        Life -= amount;
        OnLifeChanged?.Invoke();
    }

    public void OnTurnStart()
    {
        foreach (var unit in Field.Units)
            unit.MarkTurnPassed();
    }

    public void OnTurnEnd()
    {
    }
}
