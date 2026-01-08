using UnityEngine;

[CreateAssetMenu(fileName = "UnitCard", menuName = "Card/Unit")]
public class CardUnitSO : CardSO
{
    public int attack;
    public int health;

    public enum Tag
    {
        Elf,
        Beast,
        Generic,
    };

    public Tag tag;
}
