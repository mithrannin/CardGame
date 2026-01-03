using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IOwnedBy
{
    [Header("Data")]
    [SerializeField] protected CardSO cardData;
    [SerializeField] protected CardColorDisctionarySO cardColorDisctionary;

    public CardSO CardData => cardData;

    [Header("UI")]
    [SerializeField] protected TMP_Text nameText;
    [SerializeField] protected TMP_Text manaCostText;
    [SerializeField] protected TMP_Text abilityText;
    [SerializeField] protected Image manaIcon;
    [SerializeField] protected Image cardImage;
    [SerializeField] private CardVisual cardVisual;

    [Header("Runtime State")]
    public CardLocation Location { get; private set; }
    public int CurrentManaCost { get; private set; }
    private bool isDragging;
    public PlayerView Owner { get; private set; }
    public PlayerSide OwnerSide => Owner.Side;

    private Transform cachedParent;
    private Vector3 cachedWorldPosition;
    private Camera mainCamera;

    [Header("Animation Settings")]
    [SerializeField] private float moveDuration = 0.25f;
    private Coroutine moveRoutine;


    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        InitializeCard();
    }

    public void Initialize(CardSO data)
    {
        cardData = data;
        InitializeCard();
    }

    public void InitializeCard()
    {
        nameText.text = cardData.cardName;
        manaCostText.text = cardData.manaCost.ToString();
        abilityText.text = cardData.abilityDescription;
        manaIcon.sprite = cardColorDisctionary.GetIcon(cardData.manaColor);

        CurrentManaCost = cardData.manaCost;

        OnInitializeCard(cardData);
    }

    protected abstract void OnInitializeCard(CardSO data);


    #region Pointer and Drag

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!GameplayController.Instance.CanHumanInteract(this))
            return;

        CacheDragState();
        transform.SetAsLastSibling();
        cardVisual.SetSelected(true);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!GameplayController.Instance.CanHumanInteract(this))
            return;

        isDragging = true;
        cardVisual.SetRaycastBlocking(false);

        GameplayController.Instance.OnDragStarted(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        transform.position = GetMouseWorldPosition();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        isDragging = false;
        cardVisual.SetSelected(false);

        GameplayController.Instance.ResolveDrag(this);
    }

    #endregion

    #region Hover Visuals

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDragging)
            return;

        if (Location == CardLocation.Hand)
        {
            if (OwnerSide != PlayerSide.Human)
                return;

            cardVisual.SetHovered(true);
            return;
        }

        if (Location == CardLocation.Field)
        {
            if (OwnerSide == PlayerSide.AI)
                cardVisual.SetHovered(true);
        }        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDragging)
            return;

        cardVisual.SetHovered(false);
    }

    #endregion

    public void SetLocation(CardLocation newLocation)
    {
        if (Location == newLocation)
            return;

        Location = newLocation;

        if (newLocation != CardLocation.Hand)
            SetPlayable(true);

        if (newLocation == CardLocation.Graveyard)
        {
            OnEnteredGraveyard();
        }
    }
    public void SetOwner(PlayerView owner)
    {
        Owner = owner;
    }

    private void OnEnteredGraveyard()
    {
        CurrentManaCost++;
        manaCostText.text = CurrentManaCost.ToString();
    }

    public void SetSelected(bool selected)
    {
        cardVisual.SetSelected(selected);
    }

    public void SetPlayable(bool playable)
    {
        cardVisual.SetGreyedOut(!playable);
    }

    public void RestoreCachedTransform()
    {
        transform.SetParent(cachedParent);
        transform.position = cachedWorldPosition;
    }

    private void CacheDragState()
    {
        cachedParent = transform.parent;
        cachedWorldPosition = transform.position;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 screenPos = Input.mousePosition;
        screenPos.z = mainCamera.WorldToScreenPoint(transform.position).z;
        return mainCamera.ScreenToWorldPoint(screenPos);
    }

    #region Movement

    public void MoveToPoint(Vector3 targetPoint, Quaternion targetRotation, bool instantMove = false)
    {
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }

        if (instantMove)
        {
            transform.position = targetPoint;
            transform.rotation = targetRotation;
            return;
        }

        moveRoutine = StartCoroutine(MoveCoroutine(targetPoint, targetRotation, moveDuration));
    }

    private IEnumerator MoveCoroutine(Vector3 targetPoint, Quaternion targetRotation, float duration)
    {
        Vector3 startPoint = transform.position;
        Quaternion startRotation = transform.rotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            transform.position = Vector3.Lerp(startPoint, targetPoint, t);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            yield return null;
        }
        transform.position = targetPoint;
        transform.rotation = targetRotation;

        moveRoutine = null;
    }

    #endregion

}