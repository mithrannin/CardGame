using UnityEngine;

[CreateAssetMenu(fileName = "TargetLowestHealth", menuName = "Abilities/Custom/Target Lowest Health")]
public class TargetLowestHealth : AbilitySO
{
    public override void Execute(AbilityContext context)
    {
        
    }

    public CardUnit FindTarget(AbilityContext context)
    {
        CardUnit lowestHealthUnit = null;
        int lowestHealth = int.MaxValue;

        foreach (var unit in context.OpponentField.Units)
        {
            if (unit.Health > 0 && unit.Health < lowestHealthUnit.Health)
            {
                lowestHealthUnit = unit;
                lowestHealth = unit.Health;
            }  
        }
        return lowestHealthUnit;
    }
}
