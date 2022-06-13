using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ì‚ÄD‚Ì§Œä
/// </summary>
public class Discard : CardControllerBase
{
    [SerializeField] Deck m_deck;

    public void ConvartToDeck()
    {
        List<Card> cards = new List<Card>();
        for (int i = 0; i < CardParent.childCount; i++)
        {
            cards.Add(CardParent.GetChild(i).gameObject.GetComponent<Card>());
        }
        m_deck.SetCard = cards;
        //for (int i = CardParent.childCount - 1; i >= 0; i--)
        //{
        //    CardParent.GetChild(i).SetParent(m_deck.CardParent, false);
        //}
    }
}
