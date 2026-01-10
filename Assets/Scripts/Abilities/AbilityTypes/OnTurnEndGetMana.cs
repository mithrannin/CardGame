using UnityEngine;

[CreateAssetMenu(fileName = "OnTurnEndGetMana", menuName = "Abilities/OnTurnEnd/Get Mana")]
public class OnTurnEndGetMana : AbilitySO
{
    [SerializeField] private int manaGained;
    [SerializeField] private CardSO.ManaColor manaColor;

    public override void Execute(AbilityContext context)
    {
        context.Owner.Mana.AddMana(manaColor, manaGained);
    }
}
