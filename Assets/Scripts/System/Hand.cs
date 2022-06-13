using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��D�̐���
/// </summary>
public class Hand : CardControllerBase
{
    [SerializeField] Discard m_discard;

    public void ConvartToDiscard()
    {
        for (int i = CardParent.childCount - 1; i >= 0; i--)
        {
            CardParent.GetChild(i).SetParent(m_discard.CardParent, false);
        }
    }
}
