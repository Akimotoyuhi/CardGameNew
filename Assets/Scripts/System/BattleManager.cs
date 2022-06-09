using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using Cysharp.Threading.Tasks;

public class BattleManager : MonoBehaviour
{
    [SerializeField] CardData m_cardData;
    [SerializeField] Card m_cardPrefab;
    [SerializeField] Transform m_hand;
    [SerializeField] CharactorManager m_charactorManager;
    private List<Card> m_currentCard = new List<Card>();
    public CardData CardData => m_cardData;

    public void Setup()
    {
        m_charactorManager.Setup();
        Create(0);
        Create(1);
    }

    private void Create(int index)
    {
        Card c = Instantiate(m_cardPrefab);
        c.Setup(m_cardData.DataBases[index], m_charactorManager.CurrentPlayer);//とりあえず
        c.CardUsed.Subscribe(cmds => CommandExecutor(cmds)).AddTo(this);
        c.transform.SetParent(m_hand, false);
        //for (int i = 0; i < 5; i++)
        //{
        //    Card c = Instantiate(m_cardPrefab);
        //    c.Setup(m_cardData.DataBases[index], m_enemyManager.CurrentPlayer);//とりあえず
        //    c.transform.SetParent(m_hand, false);
        //}
    }

    /// <summary>
    /// フィールド効果を評価し、一部コマンドを実行する
    /// </summary>
    /// <param name="cmds"></param>
    private void CommandExecutor(List<Command> cmds)
    {
        m_charactorManager.CommandExecutor(cmds);
    }
}
