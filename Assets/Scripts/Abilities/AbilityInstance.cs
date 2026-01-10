using UnityEngine;

public class AbilityInstance
{
    public AbilitySO AbilityData { get; private set; }
    public CardUnit Source { get; private set; }
    public bool IsActive { get; set; }

    public AbilityInstance(AbilitySO abilityData, CardUnit source)
    {
        AbilityData = abilityData;
        Source = source;
        IsActive = true;
    }

    public void Execute(AbilityContext context)
    {
        if (!IsActive || !AbilityData.CanExecute(context))
            return;

        context.Source = Source;
        AbilityData.Execute(context);
    }
}
