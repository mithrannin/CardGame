using Unity.VisualScripting;
using UnityEngine;

public class PlayerContextView : MonoBehaviour
{
    public PlayerContext Context { get; private set; }

    [SerializeField] private PlayerController controller;
    [SerializeField] private HandZone hand;
    [SerializeField] private FieldZone field;
    [SerializeField] private GraveyardZone graveyard;
    [SerializeField] private PlayerMana mana;
    [SerializeField] private Deck deck;
    [SerializeField] private string playerName = "Player";

    private void Awake()
    {
        Context = new PlayerContext(
            playerName,
            controller,
            hand,
            field,
            graveyard,
            mana,
            deck
        );
    }
}
