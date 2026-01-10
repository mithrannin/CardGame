using UnityEngine;

[CreateAssetMenu(fileName = "OnSummonBuff", menuName = "Abilities/OnSummon/Buff Per Unit")]
public class OnSummonBuff : AbilitySO
{
    [SerializeField] private UnitTagSO requiredTag;
    [SerializeField] private int attackPerUnit = 1;
    [SerializeField] private int healthPerUnit = 1;
    [SerializeField] private bool countSelf = false;

    public override void Execute(AbilityContext context)
    {
        int count = CountMatchingUnits(context);

        if (count > 0)
        {
            context.Source.Attack += attackPerUnit * count;
            context.Source.Health += healthPerUnit * count;

            Debug.Log($"{context.Source.CardData.cardName} gained +{attackPerUnit * count}/+{healthPerUnit * count}");
        }
    }

    private int CountMatchingUnits(AbilityContext context)
    {
        int count = 0;

        foreach( var unit in context.OwnerField.Units)
        {
            if (!countSelf && unit == context.Source)
                continue;

            if (unit.CardData is CardUnitSO unitData && unitData.HasTag(requiredTag))
            {
                count++;
            }
        }

        return count;
    }
}
