using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public void ResolveCombat(PlayerContext attacker, PlayerContext defender)
    {
        foreach (var unit in attacker.Field.Units)
        {
            if (!unit.CanAttack)
                continue;

            ResolveAttack(unit, defender);
        }
    }

    private void ResolveAttack(CardUnit attacker, PlayerContext defender)
    {
        CardSlot opposingSlot = defender.Field.GetOpposingSlot(attacker.CurrentSlot);

        if (opposingSlot != null && opposingSlot.CardInSlot is CardUnit defenderUnit)
        {
            ResolveUnitVsUnit(attacker, defenderUnit);
        }
        else
        {
            ResolveDirectAttack(attacker, defender);
        }
    }

    private void ResolveUnitVsUnit(CardUnit attacker, CardUnit defender)
    {
        defender.TakeDamage(attacker.Attack);
    }

    private void ResolveDirectAttack(CardUnit attacker, PlayerContext defender)
    {
        defender.Controller.State.TakeDamage(attacker.Attack);
    }
}
