using System;
using TMPro;
using UnityEngine;

public class CardUnit : Card
{
    public int Attack;
    public int Health;
    public CardSlot CurrentSlot { get; private set; }

    private bool summoningSick = true;
    public bool CanAttack => !summoningSick && Health > 0 && Attack > 0;

    [Header("Unit-specific")]
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text healthText;

    protected override void OnInitializeCard(CardSO data)
    {
        if (data is not CardUnitSO)
        {
            Debug.LogError("CardUnit initialized with non-unit data!");
            return;
        }

        CardUnitSO unitData = (CardUnitSO)data;

        Attack = unitData.attack;
        Health = unitData.health;

        attackText.text = Attack.ToString();
        healthText.text = Health.ToString();
    }

    public void MarkTurnPassed()
    {
        summoningSick = false;
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Die();
        }
    }

    public event Action<CardUnit> OnDeath;
    private void Die()
    {
        OnDeath?.Invoke(this);
    }

    public void SetCurrentSlot(CardSlot slot)
    {
        CurrentSlot = slot;
    }
}
