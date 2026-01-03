using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerState State {  get; private set; }

    [field: SerializeField]
    public PlayerSide Side { get; private set; }

    [SerializeField] private HandZone hand;
    [SerializeField] private FieldZone field;
    [SerializeField] private GraveyardZone graveyard;
    public FieldZone Field { get; private set; }


    private void Awake()
    {
        State = new PlayerState(30);
        Field = field;
    }

    public void OnTurnStart()
    {
        foreach (var unit in field.Units)
            unit.MarkTurnPassed();
    }

    public void OnTurnEnd()
    {

    }
}
