using System;
using System.Collections;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    public static GameplayController Instance { get; private set; }

    [Header("Players")]
    [SerializeField] private PlayerView playerView;
    [SerializeField] private PlayerView opponentView;

    public Player Player => playerView.Player;
    public Player Opponent => opponentView.Player;

    private Player ActivePlayer => turnSystem.ActivePlayer;
    private Player InactivePlayer => turnSystem.InactivePlayer;
    public PlayerSide CurrentTurnSide => turnSystem.ActivePlayer.Side;
    public bool IsResolving { get; set; }

    [Header("Systems")]
    [SerializeField] private TurnSystemManager turnSystem;
    [SerializeField] private CardPlayResolver cardPlayResolver;
    [SerializeField] private AbilityManager abilityManager;
    
    public AIController AiController;

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
    }

    private void Start()
    {
        AiController.Initialize(Opponent, Player, turnSystem);

        StartGame();

        playerUI.Bind(Player);
        opponentUI.Bind(Opponent);
        playerActionUI.Bind(turnSystem);
        turnUI.Bind(turnSystem);


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

        if (!turnSystem.CanTakeAction(PlayerSide.Human))
            return false;

        return true;
    }

    public void OnDragStarted(Card card)
    {
        SelectCard(card);

        Player active = ActivePlayer;
        if (active.Mana.CanPayCost(card))
            HighlightValidSlots(active);
        else
            ClearHighlights(active);
    }

    public void ResolveDrag(Card card)
    {
        Player owner = (card.Owner.Player);

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

    private void HighlightValidSlots(Player context)
    {
        foreach (var slot in context.Field.Slots)
            slot.SetHighlight(slot.IsEmpty);
    }

    private void ClearHighlights(Player context)
    {
        foreach (var slot in context.Field.Slots)
            slot.SetHighlight(false);
    }

    private void RefreshHandPlayableState()
    {
        if (ActivePlayer == null || ActivePlayer.Hand == null)
            return;

        Player active = ActivePlayer;

        foreach (Card card in active.Hand.Cards)
            card.SetPlayable(active.Mana.CanPayCost(card));
    }

    #endregion

    #region Game start/end logic

    private void StartGame()
    {
        InitializePlayer(playerView);
        InitializePlayer(opponentView);

        if (UnityEngine.Random.value < 0.5f)
            turnSystem.StartGame(Player, Opponent);
        else
            turnSystem.StartGame(Opponent, Player);

        RefreshHandPlayableState();
    }

    private void InitializePlayer(PlayerView view)
    {
        Player player = view.Player;

        player.Deck.Validate();

        var manaColors = player.Deck.GetUsedManaColors();
        player.Mana.Initialize(manaColors, 2);

        player.Mana.OnManaChanged += RefreshHandPlayableState;
        player.Hand.OnHandChanged += RefreshHandPlayableState;
        player.OnLifeChanged += CheckWinCondition;

        DrawFullDeck(view);
    }

    private void DrawFullDeck(PlayerView view)
    {
        Player player = view.Player;

        foreach (var cardData in player.Deck.GetAllCards())
        {
            Card card = InstantiateCard(cardData);
            card.SetOwner(view);
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
        if (Player.Life <= 0)
            EndGame(Opponent);

        if (Opponent.Life <= 0)
            EndGame(Player);
    }

    private void EndGame(Player winner)
    {
        Debug.Log($"{winner.Name} wins!");
    }

    #endregion

    #region Handle units dying - probably move elsewhere later

    public void MoveCardToGraveyard(Card card, Player owner)
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

        Player owner = unit.Owner.Player;
        Player opponent = owner == Player ? Opponent : Player;
        abilityManager.RegisterUnit(unit, owner, opponent);
    }

    public void OnUnitRemovedFromField(CardUnit unit)
    {
        abilityManager.UnregisterUnit(unit);
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

        Player owner = unit.Owner.Player;
        MoveCardToGraveyard(unit, owner);
    }

    #endregion
}

