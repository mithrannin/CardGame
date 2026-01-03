using System.Collections;
using UnityEngine;

public class AIController : MonoBehaviour
{
    private PlayerController aiPlayer;
    private PlayerController opponent;

    public AIController(PlayerController ai, PlayerController human)
    {
        aiPlayer = ai;
        opponent = human;
    }

    public IEnumerator TakeTurn()
    {
        yield return new WaitForSeconds(0.5f);

        TryPlayCard();

        yield return new WaitForSeconds(0.5f);
    }

    private void TryPlayCard()
    {
        // Play a card from hand
    }
}
