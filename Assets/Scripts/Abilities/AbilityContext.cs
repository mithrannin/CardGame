public class AbilityContext
{
    public CardUnit Source { get; set; }
    public Player Owner { get; set; }
    public Player Opponent { get; set; }
    public FieldZone OwnerField { get; set; }
    public FieldZone OpponentField { get; set; }
    public int DamageAmount { get; set; }
    public CardUnit Target { get; set; }
}
