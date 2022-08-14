using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カードを管理するクラスの基底
/// </summary>
public class CardControllerBase : MonoBehaviour
{
    [SerializeField] GameObject m_activeObject;
    [SerializeField] Transform m_cardParent;
    protected List<Card> m_card = new List<Card>();
    public bool SetParentActive { set => m_activeObject.SetActive(value); }
    public Transform CardParent => m_cardParent;
    public int ChildCount => m_cardParent.childCount;
}
