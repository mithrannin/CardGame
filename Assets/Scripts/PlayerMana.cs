using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PlayerMana : MonoBehaviour
{
    [SerializeField] private int startingMana = 3;

    private readonly Dictionary<CardSO.ManaColor, int> manaPool = new();
    private HashSet<CardSO.ManaColor> activeColors;

    public event Action OnManaChanged;

    private void Awake()
    {
        foreach (CardSO.ManaColor color in Enum.GetValues(typeof(CardSO.ManaColor)))
        {
            manaPool[color] = startingMana;
            OnManaChanged?.Invoke();
        }
    }

    public void Initialize(HashSet<CardSO.ManaColor> colors, int startingMana)
    {
        manaPool.Clear();
        activeColors = colors;

        foreach (var color in activeColors)
        {
            manaPool[color] = startingMana;
        }

        OnManaChanged?.Invoke();
    }

    public CardSO.ManaColor GetRandomActiveColor()
    {
        int index = UnityEngine.Random.Range(0, activeColors.Count);
        return activeColors.ElementAt(index);
    }

    public bool CanPayCost(Card card)
    {
        return manaPool.TryGetValue(card.CardData.manaColor, out int availableMana) && availableMana >= card.CurrentManaCost;
    }

    public void PayCost(Card card)
    {
        if (CanPayCost(card))
        {
            manaPool[card.CardData.manaColor] -= card.CurrentManaCost;
            OnManaChanged?.Invoke();
        }
    }

    public int GetMana(CardSO.ManaColor color)
    {
        manaPool.TryGetValue(color, out int amount);
        return amount;
    }

    public IReadOnlyDictionary<CardSO.ManaColor, int> GetAllMana()
    {
        return manaPool;
    }

    public void AddMana(CardSO.ManaColor color, int amount)
    {
        if (manaPool.ContainsKey(color))
        {
            manaPool[color] += amount;
            OnManaChanged?.Invoke();
        }
        else
        {
            manaPool[color] = amount;
        }
    }
}
