using UnityEngine;

public enum TurnPhase
{
    Action,
    Combat,
    End
}

public class TurnSystemManager : MonoBehaviour
{
    public Player ActivePlayer { get; private set; }
    public Player InactivePlayer { get; private set; }
    public TurnPhase Phase { get; private set; }
    public int ActionsRemaining { get; private set; }

    [SerializeField] private CombatManager combatManager;

    private int baseActions = 1;
    private int turnNumber = 1;
    public int TurnNumber => turnNumber;


    public void StartGame(Player player, Player opponent)
    {
        ActivePlayer = player;
        InactivePlayer = opponent;
        BeginTurn();
    }

    private void BeginTurn()
    {
        ActionsRemaining = baseActions;
        if (turnNumber >= 6)
            ActionsRemaining++;

        if (turnNumber >= 12)
            ActionsRemaining++;
        
        Phase = TurnPhase.Action;
        ActivePlayer.OnTurnStart();

        if (ActivePlayer.Side == PlayerSide.AI)
        {
            StartCoroutine(GameplayController.Instance.AiController.TakeTurn());  
        }
    }

    public bool CanTakeAction(PlayerSide side)
    {
        return Phase == TurnPhase.Action && ActionsRemaining > 0 && ActivePlayer.Side == side;
    }

    public void ConsumeAction()
    {
        ActionsRemaining--;

        if (ActionsRemaining <= 0)
            BeginCombatPhase();
    }
     
    private void BeginCombatPhase()
    {
        Phase = TurnPhase.Combat;
        combatManager.ResolveCombat(ActivePlayer, InactivePlayer);
        EndTurn();
    }

    private void EndTurn()
    {
        Phase = TurnPhase.End;
        ActivePlayer.OnTurnEnd();

        (ActivePlayer, InactivePlayer) = (InactivePlayer, ActivePlayer);
        turnNumber++;

        BeginTurn();
    }
}
