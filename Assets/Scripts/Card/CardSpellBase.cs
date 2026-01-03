using UnityEngine;

public class CardSpellBase : CardSpell
{
    protected override void OnResolve(Player owner, Player opponent)
    {
        // Intentionally empty
        // Actual effect comes from CardSpellSO or effects system
    }

    protected override bool ValidateTarget(RaycastHit hit, Player owner, Player opponent)
    {
        return false;
    }

}
