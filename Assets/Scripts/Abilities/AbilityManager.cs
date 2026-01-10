using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance {  get; private set; }

    private readonly List<AbilityInstance> activeAbilities = new();
    private readonly Dictionary<CardUnit, List<AbilityInstance>> unitAbilities = new();

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterUnit(CardUnit unit, Player owner, Player opponent)
    {
        if (unit.CardData is not CardUnitSO unitData || unitData.abilities.Count == 0 || unitData.abilities == null)
        {
            return;
        }

        List<AbilityInstance> instances = new List<AbilityInstance>();

        foreach (var ability in unitData.abilities)
        {
            if (ability == null)
                continue;

            AbilityInstance instance = new AbilityInstance(ability, unit);
            instances.Add(instance);
            activeAbilities.Add(instance);
        }

        if (instances.Count > 0)
        {
            unitAbilities[unit] = instances;
            TriggerAbilities(AbilityTrigger.OnSummon, unit, owner, opponent);
        }
    }

    public void UnregisterUnit(CardUnit unit)
    {
        if (!unitAbilities.ContainsKey(unit))
            return;

        List<AbilityInstance> instances = unitAbilities[unit];

        foreach (var instance in instances)
        {
            instance.IsActive = false;
            activeAbilities.Remove(instance);
        }

        unitAbilities.Remove(unit);
    }

    public void TriggerAbilities(AbilityTrigger trigger, CardUnit source, Player owner, Player opponent, CardUnit target = null, int damageAmount = 0)
    {
        if (!unitAbilities.ContainsKey(source))
            return;

        AbilityContext context = CreateContext(source, owner, opponent, target, damageAmount);

        foreach (var instance in unitAbilities[source])
            if (instance.AbilityData.Trigger == trigger)
            {
                instance.Execute(context);
            }
    }

    public void TriggerGlobalAbilities(AbilityTrigger trigger, Player activePlayer, Player inactivePlayer)
    {
        List<AbilityInstance> toTrigger = new List<AbilityInstance>();

        foreach (var instance in activeAbilities)
        {
            if (instance.AbilityData.Trigger == trigger && instance.Source.Owner.Player == activePlayer)
            {
                toTrigger.Add(instance);
            }
        }

        foreach (var instance in toTrigger)
        {
            AbilityContext context = CreateContext(instance.Source, activePlayer, inactivePlayer);
            instance.Execute(context);
        }
    }

    public CardUnit GetCustomTarget(CardUnit attacker, Player owner, Player opponent)
    {
        if (!unitAbilities.ContainsKey(attacker))
            return null;

        foreach (var instance in unitAbilities[attacker])
        {
            if (instance.AbilityData.Trigger == AbilityTrigger.CustomTargeting)
            {
                if (instance.AbilityData is TargetLowestHealth targetAbility)
                {
                    AbilityContext context = CreateContext(attacker, owner, opponent);
                    return targetAbility.FindTarget(context);
                }
            }
        }

        return null;
    }

    private AbilityContext CreateContext(CardUnit source, Player owner, Player opponent, CardUnit target = null, int damageAmount = 0)
    {
        return new AbilityContext
        {
            Source = source,
            Owner = owner,
            Opponent = opponent,
            OwnerField = owner.Field,
            OpponentField = opponent.Field,
            Target = target,
            DamageAmount = damageAmount
        };
    }
}
