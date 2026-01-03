using UnityEngine;

public class CardSpellBase : CardSpell
{
    protected override void OnResolve(PlayerContext owner, PlayerContext opponent)
    {
        // Intentionally empty
        // Actual effect comes from CardSpellSO or effects system
    }

    protected override bool ValidateTarget(RaycastHit hit, PlayerContext owner, PlayerContext opponent)
    {
        return false;
    }

}
