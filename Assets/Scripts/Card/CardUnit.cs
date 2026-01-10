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
    [SerializeField] private TMP_Text tagText;
    public Animator animator;

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

        tagText.text = unitData.GetTagsDisplayText();
    }

    public void MarkTurnPassed()
    {
        summoningSick = false;
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        animator.SetTrigger("Hurt");
        if (Health <= 0)
        {
            Die();
        }
        healthText.text = Health.ToString();
    }

    public void Heal(int amount)
    {
        if (cardData is not CardUnitSO unitData)
            return;

        int maxHealth = unitData.health;
        Health = Mathf.Min(Health + amount, maxHealth);
        healthText.text = Health.ToString();
        
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
