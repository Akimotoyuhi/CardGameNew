using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �f�b�L����J�[�h�����O���鎞�Ɏg���\��
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
