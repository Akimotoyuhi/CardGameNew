using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��D�A�R�D�A�̂ĎD�̊��N���X
/// </summary>
public class CardControllerBase : MonoBehaviour
{
    [SerializeField] Transform m_cardParent;
    protected List<Card> m_card = new List<Card>();
    public Transform CardParent => m_cardParent;
    public int ChildCount => m_cardParent.childCount;
}
