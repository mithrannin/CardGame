using UnityEngine;

[CreateAssetMenu(fileName = "ManaColor", menuName = "Card/Misc/Mana Color Dictionary")]
public class CardColorDisctionarySO : ScriptableObject
{
    public ManaColorEntry[] manaColors;

    [System.Serializable]
    public struct ManaColorEntry
    {
        public CardSO.ManaColor color;
        public Sprite icon;
        public Color backgroundColor;
    }

    public Sprite GetIcon(CardSO.ManaColor color)
    {
        foreach (var manaColor in manaColors)
        {
            if (manaColor.color == color)
            {
                return manaColor.icon;
            }
        }
        return null;
    }

    public Color GetBackgroundColor(CardSO.ManaColor color)
    {
        foreach(var manaColor in manaColors)
        {
            if (manaColor.color == color)
            {
                return manaColor.backgroundColor;
            }
        }
        return Color.brown;
    }
}
