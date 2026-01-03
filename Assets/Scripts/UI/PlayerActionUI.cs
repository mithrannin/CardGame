using UnityEngine;
using UnityEngine.UI;

public class PlayerActionUI : MonoBehaviour
{
    [SerializeField] private TurnSystemManager turnSystem;

    [Header("Buttons")]
    [SerializeField] private Button resurrectButton;
    [SerializeField] private Button gainManaButton;

    public void Bind(TurnSystemManager turnSystem)
    {
        this.turnSystem = turnSystem;

        resurrectButton.onClick.AddListener(OnResurrectClicked);
        gainManaButton.onClick.AddListener(OnGainManaClicked);
    }

    private void OnResurrectClicked()
    {
        if (turnSystem.ActivePlayer.Side == PlayerSide.Human)        
            PlayerActions.TryReturnGraveyardToHand(turnSystem.ActivePlayer, turnSystem);
    }

    private void OnGainManaClicked()
    {
        if (turnSystem.ActivePlayer.Side == PlayerSide.Human)
            PlayerActions.TryGainMana(turnSystem.ActivePlayer, turnSystem);
    }
}
