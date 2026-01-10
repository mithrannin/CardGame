using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class CardUnit : Card
{
    public int Attack;
    public int Health;

    private int baseAttack;
    private int baseHealth;

    public CardSlot CurrentSlot { get; private set; }

    private bool summoningSick = true;
    public bool CanAttack => !summoningSick && Health > 0 && Attack > 0;

    [Header("Unit-specific")]
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text tagText;
    public Animator animator;

    [Header("Stat Colors")]
    [SerializeField] private Color buffedColor = Color.green;
    [SerializeField] private Color debuffedColor = Color.red;
    [SerializeField] private Color normalColor = Color.white;

    protected override void OnInitializeCard(CardSO data)
    {
        if (data is not CardUnitSO)
        {
            Debug.LogError("CardUnit initialized with non-unit data!");
            return;
        }

        CardUnitSO unitData = (CardUnitSO)data;

        baseAttack = unitData.attack;
        baseHealth = unitData.health;
        Attack = unitData.attack;
        Health = unitData.health;


        attackText.text = Attack.ToString();
        healthText.text = Health.ToString();

        if (unitData.abilities != null && unitData.abilities.Count == 1)
        {
            abilityText.text = unitData.abilities[0].Description;
        }
        
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
        UpdateStats();
    }

    public void Heal(int amount)
    {
        if (cardData is not CardUnitSO unitData)
            return;

        int maxHealth = unitData.health;
        Health = Mathf.Min(Health + amount, maxHealth);
        UpdateStats();
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

    public void UpdateStats()
    {
        healthText.text = Health.ToString();
        attackText.text = Attack.ToString();

        UpdateStatColor(attackText, Attack, baseAttack);
        UpdateStatColor(healthText, Health, baseHealth);

        attackText.transform.DOPunchScale(Vector3.one * 1f, 0.6f);
        healthText.transform.DOPunchScale(Vector3.one * 1f, 0.6f);
    }

    private void UpdateStatColor(TMP_Text textComponent, int currentValue, int baseValue)
    {
        if (currentValue > baseValue)
        {
            textComponent.color = buffedColor;
        }
        else if (currentValue < baseValue)
        {
            textComponent.color = debuffedColor;
        }
        else
        {
            textComponent.color = normalColor;
        }
    }
}
