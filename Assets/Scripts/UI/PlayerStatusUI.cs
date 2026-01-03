using TMPro;
using UnityEngine;

public class PlayerStatusUI : MonoBehaviour
{
    [SerializeField] private TMP_Text lifeText;
    [SerializeField] private TMP_Text graveyardCountText;

    [System.Serializable]
    public struct ManaEntry
    {
        public CardSO.ManaColor color;
        public TMP_Text text;
    }

    [SerializeField] private ManaEntry[] manaEntries;

    private Player context;

    public void Bind(Player context)
    {
        this.context = context;

        context.Mana.OnManaChanged += RefreshMana;
        context.OnLifeChanged += RefreshLife;
        context.Graveyard.OnChanged += RefreshGraveyard;

        RefreshAll();
    }

    private void RefreshAll()
    {
        RefreshMana();
        RefreshLife();
        RefreshGraveyard();
    }

    private void RefreshMana()
    {
        foreach (var entry in manaEntries)
        {
            bool active = context.Mana.GetAllMana().ContainsKey(entry.color);
            entry.text.gameObject.SetActive(active);

            if (active)
                entry.text.text = context.Mana.GetMana(entry.color).ToString();
        }
    }

    private void RefreshLife()
    {
        lifeText.text = "LIFE: " + context.Life.ToString();
    }

    private void RefreshGraveyard()
    {
        graveyardCountText.text = context.Graveyard.Cards.Count.ToString();
    }
}
