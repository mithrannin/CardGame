using System;
using System.Collections;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    public static GameplayController Instance { get; private set; }

    [Header("Players")]
    [SerializeField] private PlayerContextView playerView;
    [SerializeField] private PlayerContextView opponentView;

    private PlayerContext player;
    private PlayerContext opponent;

    private PlayerContext ActivePlayer => turnSystem.ActivePlayer;
    private PlayerContext InactivePlayer => turnSystem.InactivePlayer;
    public PlayerSide CurrentTurnSide => turnSystem.ActivePlayer.Controller.Side;
    public bool IsResolving { get; set; }

    [Header("Systems")]
    [SerializeField] private TurnSystemManager turnSystem;
    [SerializeField] private CardPlayResolver cardPlayResolver;
    [SerializeField] private PlayerActionController actionController;

    [Header("UI")]
    [SerializeField] private PlayerStatusUI playerUI;
    [SerializeField] private PlayerStatusUI opponentUI;
    [SerializeField] private PlayerActionUI playerActionUI;
    [SerializeField] private TurnUI turnUI;

    [Header("Card Prefabs")]
    [SerializeField] private CardUnit cardUnitPrefab;
    [SerializeField] private CardSpell cardSpellPrefab;

    private Card selectedCard;
    public Card SelectedCard => selectedCard;

    private void Awake()
    {
        Instance = this;

        player = playerView.Context;
        opponent = opponentView.Context;
    }

    private void Start()
    {
        StartGame();

        playerUI.Bind(player);
        opponentUI.Bind(opponent);
        playerActionUI.Bind(turnSystem);
        turnUI.Bind(turnSystem);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (turnSystem.CanTakeAction())
            {
                turnSystem.ConsumeAction();
                Debug.Log($"Active player: {turnSystem.ActivePlayer}, turn phase: {turnSystem.Phase}");
            }
        }
    }

    #region Card Dragging (called by Card)

    public bool CanHumanInteract(IOwnedBy owned)
    {
        if (IsResolving)
            return false;

        if (CurrentTurnSide != PlayerSide.Human)
            return false;

        if (owned.OwnerSide != PlayerSide.Human) 
            return false;

        if (!turnSystem.CanTakeAction())
            return false;

        return true;
    }

    public void OnDragStarted(Card card)
    {
        SelectCard(card);

        PlayerContext active = ActivePlayer;
        if (active.Mana.CanPayCost(card))
            HighlightValidSlots(active);
        else
            ClearHighlights(active);
    }

    public void ResolveDrag(Card card)
    {
        PlayerContext owner = GetPlayerContext(card.Owner);

        ClearHighlights(owner);
        DeselectCard();

        if (!CanHumanInteract(card))
        {
            owner.Hand.RestoreCardPosition(card);
            return;
        }

        if (cardPlayResolver.TryResolveDrop(card, ActivePlayer, InactivePlayer))
        {
            turnSystem.ConsumeAction();
            return;
        }

        owner.Hand.RestoreCardPosition(card);
    }

    #endregion

    #region Selection & UI

    private void SelectCard(Card card)
    {
        if (selectedCard == card)
            return;

        DeselectCard();
        selectedCard = card;
        card.SetSelected(true);
    }

    private void DeselectCard()
    {
        if (selectedCard == null)
            return;

        selectedCard.SetSelected(false);
        selectedCard = null;
    }

    private void HighlightValidSlots(PlayerContext context)
    {
        foreach (var slot in context.Field.Slots)
            slot.SetHighlight(slot.IsEmpty);
    }

    private void ClearHighlights(PlayerContext context)
    {
        foreach (var slot in context.Field.Slots)
            slot.SetHighlight(false);
    }

    private void RefreshHandPlayableState()
    {
        if (ActivePlayer == null || ActivePlayer.Hand == null)
            return;

        PlayerContext active = ActivePlayer;

        foreach (Card card in active.Hand.Cards)
            card.SetPlayable(active.Mana.CanPayCost(card));
    }

    #endregion

    #region Game start/end logic

    private void StartGame()
    {
        InitializePlayer(player);
        InitializePlayer(opponent);

        if (UnityEngine.Random.value < 0.5f)
            turnSystem.StartGame(player, opponent);
        else
            turnSystem.StartGame(opponent, player);
    }

    private void InitializePlayer(PlayerContext context)
    {
        context.Deck.Validate();

        var manaColors = context.Deck.GetUsedManaColors();
        context.Mana.Initialize(manaColors, 2);

        context.Mana.OnManaChanged += RefreshHandPlayableState;
        context.Hand.OnHandChanged += RefreshHandPlayableState;
        context.Controller.State.OnLifeChanged += CheckWinCondition;

        DrawFullDeck(context);

        StartCoroutine(DelayedHandRefresh());
    }

    private IEnumerator DelayedHandRefresh()
    {
        yield return null;
        RefreshHandPlayableState();
    }

    private void DrawFullDeck(PlayerContext player)
    {
        foreach (var cardData in player.Deck.GetAllCards())
        {
            Card card = InstantiateCard(cardData);
            card.SetOwner(player.Controller);
            player.Hand.AddCard(card);
        }
    }

    public Card InstantiateCard(CardSO cardData)
    {
        Card card;
        if (cardData is CardUnitSO)
        {
            card = Instantiate(cardUnitPrefab);
        }
        else if (cardData is CardSpellSO)
        {
            card = Instantiate(cardSpellPrefab);
        }
        else
        {
            Debug.LogError($"Unsupported CardSO type: {cardData.name}");
            return null;
        }

        card.Initialize(cardData);
        return card;
    }

    public void CheckWinCondition()
    {
        if (player.Controller.State.Life <= 0)
            EndGame(opponent.Controller);

        if (opponent.Controller.State.Life <= 0)
            EndGame(player.Controller);
    }

    private void EndGame(PlayerController winner)
    {
        PlayerContext winnerContext = GetPlayerContext(winner);
        Debug.Log($"{winnerContext.Name} wins!");
    }

    #endregion

    #region Handle units dying - probably move elsewhere later

    public void MoveCardToGraveyard(Card card, PlayerContext owner)
    {
        if (card is CardUnit unit && unit.CurrentSlot != null)
        {
            unit.CurrentSlot.ClearSlot();
            unit.SetCurrentSlot(null);
        }

        owner.Graveyard.AddCard(card);
    }

    public void OnUnitPlayed(CardUnit unit)
    {
        RegisterUnit(unit);
    }

    public void OnUnitRemovedFromField(CardUnit unit)
    {
        UnregisterUnit(unit);
    }

    private void RegisterUnit(CardUnit unit)
    {
        unit.OnDeath += HandleUnitDeath;
    }

    private void UnregisterUnit(CardUnit unit)
    {
        unit.OnDeath -= HandleUnitDeath;
    }

    private void HandleUnitDeath(CardUnit unit)
    {
        UnregisterUnit(unit);

        PlayerContext owner = GetPlayerContext(unit.Owner);
        MoveCardToGraveyard(unit, owner);
    }

    public PlayerContext GetPlayerContext(PlayerController controller)
    {
        if (controller == player.Controller)
            return player;

        if (controller == opponent.Controller)
            return opponent;

        throw new Exception("Unknown player controller");
    }

    #endregion
}

