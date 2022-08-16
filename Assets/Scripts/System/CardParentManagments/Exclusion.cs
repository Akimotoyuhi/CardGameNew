using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// デッキからカードを除外する時に使う予定
/// </summary>
public class Exclusion : CardControllerBase
{
    [SerializeField] Deck m_deck;

    public Card SetCard
    {
        set
        {
            value.transform.SetParent(CardParent, false);
        }
    }
}
