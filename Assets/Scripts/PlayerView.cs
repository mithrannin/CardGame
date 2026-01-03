using UnityEngine;

public class PlayerView : MonoBehaviour
{
    public Player Player { get; private set; }

    [field: SerializeField]
    public PlayerSide Side { get; private set; }

    [SerializeField] private HandZone hand;
    [SerializeField] private FieldZone field;
    [SerializeField] private GraveyardZone graveyard;
    [SerializeField] private PlayerMana mana;
    [SerializeField] private Deck deck;
    [SerializeField] private string playerName = "Player";

    private void Awake()
    {
        Player = new Player(
            playerName,
            Side,
            hand,
            field,
            graveyard,
            mana,
            deck
        );
    }
}
