using UnityEngine;

public enum TurnPhase
{
    Action,
    Combat,
    End
}

public class TurnSystemManager : MonoBehaviour
{
    public PlayerContext ActivePlayer { get; private set; }
    public PlayerContext InactivePlayer { get; private set; }

    public TurnPhase Phase { get; private set; }
    public int ActionsRemaining { get; private set; }

    [SerializeField] private CombatManager combatManager;

    private int baseActions = 3;
    private int turnNumber = 1;
    public int TurnNumber => turnNumber;


    public void StartGame(PlayerContext player, PlayerContext opponent)
    {
        ActivePlayer = player;
        InactivePlayer = opponent;
        BeginTurn();
    }

    private void BeginTurn()
    {
        ActionsRemaining = baseActions;
        Phase = TurnPhase.Action;

        ActivePlayer.Controller.OnTurnStart();
    }

    public bool CanTakeAction()
    {
        return Phase == TurnPhase.Action && ActionsRemaining > 0;
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
        ActivePlayer.Controller.OnTurnEnd();

        (ActivePlayer, InactivePlayer) = (InactivePlayer, ActivePlayer);
        turnNumber++;

        BeginTurn();
    }
}
