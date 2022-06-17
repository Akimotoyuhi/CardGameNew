using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;

public class BattleManager : MonoBehaviour
{
    [SerializeField] CardData m_commonCardData;
    [SerializeField] CardData m_originalCardData;
    [SerializeField] CardData m_akCardData;
    [SerializeField] Card m_cardPrefab;
    [SerializeField] EncountData m_encountData;
    [SerializeField] Hand m_hand;
    [SerializeField] Deck m_deck;
    [SerializeField] Discard m_discard;
    [SerializeField] CharactorManager m_charactorManager;
    private int m_currentTurn;
    private List<Card> m_currentCard = new List<Card>();
    private CardData m_useCardData;
    private ReactiveProperty<BattleState> m_battleState = new ReactiveProperty<BattleState>();
    /// <summary>バトルの状態遷移を通知する</summary>
    public System.IObservable<BattleState> BattleStateObservable => m_battleState;

    public void Setup()
    {
        m_charactorManager.Setup();
        m_charactorManager.CurrentEnemies.ForEach(e =>
        e.ActionSubject.Subscribe(c => CommandExecutor(c)).AddTo(e));
        m_deck.SetParentActive = false;
        m_discard.SetParentActive = false;
    }

    private void Create()
    {
        //使用するデータを設定
        CardClassType cct = m_charactorManager.CardClassType;
        switch (cct)
        {
            case CardClassType.Common:
                m_useCardData = m_commonCardData;
                break;
            case CardClassType.Original:
                m_useCardData = m_originalCardData;
                break;
            case CardClassType.AK:
                m_useCardData = m_akCardData;
                break;
            default:
                throw new System.Exception("使用するカードデータが見つかりませんでした");
        }

        //生成
        List<int> list = m_charactorManager.CardClass.GetCardID(cct);
        List<Card> toDeckCards = new List<Card>();
        list.ForEach(i =>
        {
            Card c = Instantiate(m_cardPrefab);
            c.Setup(m_useCardData.DataBases[i], m_charactorManager.CurrentPlayer);
            c.CardUsed.Subscribe(cmds =>
            {
                CommandExecutor(cmds);
                c.transform.SetParent(m_discard.CardParent, false);
            })
            .AddTo(c);
            toDeckCards.Add(c);
            m_currentCard.Add(c);
        });
        m_deck.SetCard = toDeckCards;
        m_deck.Draw(m_charactorManager.CurrentPlayer.DrowNum);
    }

    /// <summary>ボタンが押された後の一連の流れ</summary>
    public async void OnBattle()
    {
        m_hand.ConvartToDiscard();
        m_battleState.Value = BattleState.EnemyFaze;
        Debug.Log("ボタンが押された");
        await m_charactorManager.TurnEnd(m_currentTurn);
        m_battleState.Value = BattleState.PlayerFaze;
        m_currentTurn++;
        m_deck.Draw(m_charactorManager.CurrentPlayer.DrowNum);
        await m_charactorManager.TurnBegin(m_currentTurn);
    }

    /// <summary>
    /// フィールド効果を評価し、一部コマンドを実行する
    /// </summary>
    /// <param name="cmds"></param>
    private void CommandExecutor(List<Command> cmds)
    {
        m_charactorManager.CommandExecutor(cmds);
    }

    public void Encount(MapID mapID, CellType cellType)
    {
        List<EnemyID> e = m_encountData.GetEncountData(mapID).GetEnemies(cellType);
        m_charactorManager.Create(e);
        Create();
        m_battleState.Value = BattleState.PlayerFaze;
        m_charactorManager.TurnBegin(m_currentTurn).Forget();
    }
}

public enum BattleState
{
    None,
    EnemyFaze,
    PlayerFaze,
}