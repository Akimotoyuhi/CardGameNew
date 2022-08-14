using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// èD‚Ì§Œä
/// </summary>
public class Hand : CardControllerBase
{
    [SerializeField] Discard m_discard;

    public void ConvartToDiscard()
    {
        for (int i = CardParent.childCount - 1; i >= 0; i--)
        {
            CardParent.GetChild(i).gameObject.GetComponent<Card>().SetParent(m_discard.CardParent, false);
            //CardParent.GetChild(i).SetParent(m_discard.CardParent, false);
        }
    }
}
