using System.Collections;
using UnityEngine;

public abstract class CardSpell : Card
{
    [Header("Spell-specific")]
    [SerializeField] private float resolveDelay = 0.5f;

    protected override void OnInitializeCard(CardSO data)
    {
        if (data is not CardSpellSO)
        {
            Debug.LogError("CardSpell initialized with non-spell data!");
        }
    }

    public bool CanResolveOn(RaycastHit hit, Player owner, Player opponent)
    {
        return ValidateTarget(hit, owner, opponent);
    }

    public void ResolveSpell(Player owner, Player opponent, RaycastHit hit)
    {
        StartCoroutine(ResolveSpellRoutine(owner, opponent, hit));
    }

    private IEnumerator ResolveSpellRoutine(Player owner, Player opponent, RaycastHit hit)
    {
        MoveToPoint(Vector3.zero, Quaternion.identity);

        yield return new WaitForSeconds(resolveDelay);

        OnResolve(owner, opponent);

        yield return new WaitForSeconds(0.25f);

        GameplayController.Instance.MoveCardToGraveyard(this, owner);
    }

    protected abstract bool ValidateTarget(RaycastHit hit, Player owner, Player opponent);

    protected abstract void OnResolve(Player owner, Player opponent);
}
