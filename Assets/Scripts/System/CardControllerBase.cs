using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 手札、山札、捨て札の基底クラス
/// </summary>
public class CardControllerBase : MonoBehaviour
{
    [SerializeField] Transform m_cardParent;
    public Transform CardParent => m_cardParent;
    public int ChildCount => m_cardParent.childCount;
    public virtual void Setup()
    {

    }
}
