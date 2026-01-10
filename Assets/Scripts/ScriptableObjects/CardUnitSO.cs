using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "UnitCard", menuName = "Card/Unit")]
public class CardUnitSO : CardSO
{
    public int attack;
    public int health;

    [Header("Tags")]
    public List<UnitTagSO> tags = new ();

    [Header("Abilities")]
    public List <AbilitySO> abilities = new ();

    public bool HasTag(UnitTagSO tag)
    {
        return tags.Contains(tag);
    }

    public string GetTagsDisplayText()
    {
        if (tags == null || tags.Count == 0)
            return string.Empty;

        List<string> tagNames = new List<string>();
        foreach (var tag in tags)
        {
            if (tag != null)
                tagNames.Add(tag.TagName);
        }

        return string.Join(", ", tagNames);
    }

}
