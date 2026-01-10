using System.Collections;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [SerializeField] private AbilityManager abilityManager;

    public void ResolveCombat(Player attacker, Player defender)
    {
        StartCoroutine(ResolveCombatCoroutine(attacker, defender));
    }

    private void ResolveAttack(CardUnit attacker, Player defender)
    {
        Player attackerOwner = attacker.Owner.Player;

        CardUnit customTarget = abilityManager.GetCustomTarget(attacker, attackerOwner, defender);
        attacker.animator.SetTrigger("Attack");

        if (customTarget != null)
        {
            ResolveUnitVsUnit(attacker, customTarget);
        }
        else
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

        abilityManager.TriggerAbilities(AbilityTrigger.OnAttack, attacker, attackerOwner, defender);
    }

    private IEnumerator ResolveCombatCoroutine(Player attacker, Player defender)
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

    private void ResolveDirectAttack(CardUnit attacker, Player defender)
    {
        defender.TakeDamage(attacker.Attack);
    }
}
