using TMPro;
using UnityEngine;

public class TurnUI : MonoBehaviour
{
    [SerializeField] private TMP_Text turnNumberText;
    [SerializeField] private TMP_Text phaseText;
    [SerializeField] private TMP_Text actionsText;

    private TurnSystemManager turnSystem;

    public void Bind(TurnSystemManager turnSystem)
    {
        this.turnSystem = turnSystem;
        Refresh();
    }

    private void Update()
    {
        //Should replace the Update here with something event-based
        if (turnSystem == null)
            return;

        Refresh();
    }

    private void Refresh()
    {
        turnNumberText.text = $"Turn: {turnSystem.TurnNumber}";
        phaseText.text = turnSystem.ActivePlayer.Name;
        actionsText.text = $"Actions: {turnSystem.ActionsRemaining}";
    }
}
