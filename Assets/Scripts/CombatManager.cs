using System.Collections;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public void ResolveCombat(PlayerContext attacker, PlayerContext defender)
    {
        StartCoroutine(ResolveCombatCoroutine(attacker, defender));
    }

    private void ResolveAttack(CardUnit attacker, PlayerContext defender)
    {
        CardSlot opposingSlot = defender.Field.GetOpposingSlot(attacker.CurrentSlot);
        attacker.animator.SetTrigger("Attack");

        if (opposingSlot != null && opposingSlot.CardInSlot is CardUnit defenderUnit)
        {
            ResolveUnitVsUnit(attacker, defenderUnit);
        }
        else
        {
            ResolveDirectAttack(attacker, defender);
        }
    }

    private IEnumerator ResolveCombatCoroutine(PlayerContext attacker, PlayerContext defender)
    {
        GameplayController.Instance.IsResolving = true;

        foreach (var unit in attacker.Field.Units)
        {
            if (!unit.CanAttack)
                continue;

            ResolveAttack(unit, defender);

            yield return new WaitForSeconds(0.4f);
        }

        GameplayController.Instance.IsResolving = false;
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
