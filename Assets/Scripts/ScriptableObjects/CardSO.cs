using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card/New Card")]
public abstract class CardSO : ScriptableObject
{
    public string cardName;
    public int manaCost;
    public enum ManaColor
    {
        White,
        Blue,
        Black,
        Red,
        Green,
    };

    public Sprite cardArt;
    public ManaColor manaColor;

    [TextArea]
    public string abilityDescription;
}