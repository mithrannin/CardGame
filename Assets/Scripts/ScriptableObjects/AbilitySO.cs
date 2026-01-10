using UnityEngine;

public abstract class AbilitySO : ScriptableObject
{
    [SerializeField] private string abilityName;
    [SerializeField] private AbilityTrigger trigger;
    [TextArea(2, 4)]
    [SerializeField] private string description;

    public string AbilityName => abilityName;
    public AbilityTrigger Trigger => trigger;
    public string Description => description;

    public abstract void Execute(AbilityContext context);

    public virtual bool CanExecute(AbilityContext context)
    {
        return true;
    }
}
