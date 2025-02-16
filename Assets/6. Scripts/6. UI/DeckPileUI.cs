using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; // ✅ Import TextMeshPro namespace

public class DeckPileUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private DeckManager deckManager; // Reference to deck manager
    [SerializeField] private TMP_Text pileText; // ✅ Updated to TMP_Text
    [SerializeField] private bool isDrawPile; // True = Draw pile, False = Discard pile

    private void Start()
    {
        if (deckManager == null)
        {
            deckManager = FindObjectOfType<DeckManager>();
        }
        if (pileText == null)
        {
            Debug.LogError("[DeckPileUI] ❌ No TMP_Text component assigned!");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowPileContents();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ClearPileContents();
    }

    private void ShowPileContents()
    {
        if (pileText == null || deckManager == null) return;

        List<BaseCard> pile = isDrawPile ? deckManager.DrawPile as List<BaseCard> : deckManager.DiscardPile as List<BaseCard>;

        if (pile.Count == 0)
        {
            pileText.text = isDrawPile ? "Draw Pile: Empty" : "Discard Pile: Empty";
            return;
        }

        pileText.text = $"{(isDrawPile ? "Draw Pile" : "Discard Pile")}:\n";
        foreach (var card in pile)
        {
            pileText.text += $"{card.CardName}\n";
        }
    }

    private void ClearPileContents()
    {
        if (pileText != null)
        {
            pileText.text = "";
        }
    }
}
