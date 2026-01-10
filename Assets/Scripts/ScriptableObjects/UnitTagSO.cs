using UnityEngine;

[CreateAssetMenu(fileName = "UnitTag", menuName = "Card/Unit Tag")]
public class UnitTagSO : ScriptableObject
{
    [SerializeField] private string tagName;
    [TextArea(2,3)] private string tagDescription;

    public string TagName => tagName;
    public string TagDescription => tagDescription;

    public override string ToString()
    {
        return tagName;
    }
}
