using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] CardData m_cardData;
    [SerializeField] Card m_cardPrefab;
    [SerializeField] Transform m_hand;
    [SerializeField] CharactorManager m_enemyManager;
    private List<Card> m_currentCard = new List<Card>();
    public CardData CardData => m_cardData;

    public void Setup()
    {
        m_enemyManager.Setup();
        Create();
    }

    private void Create()
    {
        for (int i = 0; i < 5; i++)
        {
            Card c = Instantiate(m_cardPrefab);
            c.Setup(m_cardData.DataBases[0]);//とりあえず
            c.transform.SetParent(m_hand, false);
        }
    }
}
