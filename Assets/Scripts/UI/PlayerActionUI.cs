using UnityEngine;
using UnityEngine.UI;

public class PlayerActionUI : MonoBehaviour
{
    [SerializeField] private PlayerActionController actionController;
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
        if (turnSystem.ActivePlayer.Controller.Side == PlayerSide.Human)        
            actionController.TryReturnGraveyardToHand(turnSystem.ActivePlayer);
    }

    private void OnGainManaClicked()
    {
        if (turnSystem.ActivePlayer.Controller.Side == PlayerSide.Human)
            actionController.TryGainMana(turnSystem.ActivePlayer);
    }
}
