using System;
using UnityEngine;

public class PlayerState
{
    public int Life {  get; private set; }
    public event Action OnLifeChanged;

    public PlayerState(int startingLife)
    {
        Life = startingLife;
    }

    public void TakeDamage(int amount)
    {
        Life -= amount;
        OnLifeChanged?.Invoke();
    }
}
