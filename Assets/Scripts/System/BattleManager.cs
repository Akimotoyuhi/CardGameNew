using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// バトルの管理クラス
/// </summary>
public class BattleManager : MonoBehaviour
{
    /// <summary>
    /// カードデータの集まり
    /// </summary>
    [System.Serializable]
    public class CardClassDatas
    {
        [SerializeField] CardData m_commonCardData;
        [SerializeField] CardData m_originalCardData;
        [SerializeField] CardData m_akCardData;
        /// <summary>
        /// 任意のカードのデータの集まりが取れる
        /// </summary>
        /// <param name="cardClassType"></param>
        /// <returns></returns>
        public CardData GetData(CardClassType cardClassType)
        {
            switch (cardClassType)
            {
                case CardClassType.Common:
                    return m_commonCardData;
                case CardClassType.Original:
                    return m_originalCardData;
                case CardClassType.AK:
                    return m_akCardData;
                default:
                    return null;
            }

        }
        /// <summary>
        /// 任意のカードのデータが取れる
        /// </summary>
        /// <param name="haveCardData"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public CardDataBase GetDataBase(HaveCardData haveCardData)
        {
            switch (haveCardData.CardCalssType)
            {
                case CardClassType.Common:
                    return m_commonCardData.DataBases[haveCardData.CardID].GetCardData(haveCardData.IsUpGrade);
                case CardClassType.Original:
                    return m_originalCardData.DataBases[haveCardData.CardID].GetCardData(haveCardData.IsUpGrade);
                case CardClassType.AK:
                    return m_akCardData.DataBases[haveCardData.CardID].GetCardData(haveCardData.IsUpGrade);
                default:
                    throw new System.Exception("存在しないデータにアクセスしようとしています");
            }
        }
    }
    [SerializeField] Card m_cardPrefab;
    [SerializeField] EncountData m_encountData;
    [SerializeField] Hand m_hand;
    [SerializeField] Deck m_deck;
    [SerializeField] Discard m_discard;
    [SerializeField] CharactorManager m_charactorManager;
    [SerializeField] CardClassDatas m_cardDatas;
    [SerializeField] int m_rewardNum;
    private int m_currentTurn;
    /// <summary>この戦闘中のカードのインスタンス</summary>
    private List<Card> m_currentCard = new List<Card>();
    /// <summary>戦闘終了を通知する</summary>
    private Subject<Unit> m_battleFinished = new Subject<Unit>();
    private ReactiveProperty<BattleState> m_battleState = new ReactiveProperty<BattleState>();
    /// <summary>現在の戦闘のBattleType</summary>
    private BattleType m_currentBattleType;
    /// <summary>バトルの状態遷移を通知する</summary>
    public System.IObservable<BattleState> BattleStateObservable => m_battleState;
    /// <summary>バトルの終了を通知する</summary>
    public System.IObservable<Unit> BattleFinished => m_battleFinished;

    public void Setup()
    {
        //各カードデータのID割り振り
        m_cardDatas.GetData(CardClassType.Common).Setup();
        m_cardDatas.GetData(CardClassType.AK).Setup();
        m_cardDatas.GetData(CardClassType.Original).Setup();

        //キャラクターマネージャーのセットアップと通知の購読
        m_charactorManager.Setup();
        m_charactorManager.NewEnemyCreateSubject
            .Subscribe(e =>
            {
                e.ActionSubject
                .Subscribe(cmd => CommandExecutor(cmd))
                .AddTo(e);
            })
            .AddTo(m_charactorManager);
        m_charactorManager.BattleEndSubject
            .Subscribe(type => BattleEnd(type))
            .AddTo(m_charactorManager);

        //デッキと捨て札一覧画面を消す
        m_deck.SetParentActive = false;
        m_discard.SetParentActive = false;
    }

    private void Create()
    {
        //HaveCardからカードを生成させる
        m_charactorManager.HaveCard.ForEach(card =>
        {
            Card c = Instantiate(m_cardPrefab);
            c.Setup(m_cardDatas.GetDataBase(card),
                m_cardDatas.GetData(card.CardCalssType).GetRaritySprite,
                m_cardDatas.GetData(card.CardCalssType).GetTypeSprite,
                m_charactorManager.CurrentPlayer);
            c.CardExecute.Subscribe(cmds =>
            {
                CommandExecutor(cmds);
                c.transform.SetParent(m_discard.CardParent, false);
            }).AddTo(c);
            m_currentCard.Add(c);
        });
        m_deck.SetCard = m_currentCard;
        m_deck.Draw(m_charactorManager.CurrentPlayer.DrowNum);
    }

    /// <summary>ターン終了ボタンが押された後の一連の流れ</summary>
    public async void OnBattle()
    {
        Debug.Log("ボタンが押された");
        //手札を全て捨てて敵のターンを開始
        m_hand.ConvartToDiscard();
        m_battleState.Value = BattleState.EnemyFaze;
        await m_charactorManager.TurnEnd(m_currentTurn);

        //敵のターンが終わったらターン数を加算しプレイヤーのターンに移る
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
        //この辺でフィールド効果を評価する予定
        m_charactorManager.CommandExecutor(cmds);
    }

    /// <summary>
    /// 戦闘開始
    /// </summary>
    public void Encount(MapID mapID, CellType cellType)
    {
        if (cellType == CellType.FirstHalfBattle || cellType == CellType.SecondHalfBattle)
            m_currentBattleType = BattleType.Normal;
        else if (cellType == CellType.Elite)
            m_currentBattleType = BattleType.Elite;
        else if (cellType == CellType.Boss)
            m_currentBattleType = BattleType.Boss;

        List<EnemyID> e = m_encountData.GetEncountData(mapID).GetEnemies(cellType);
        m_charactorManager.Create(e);
        Create();
        m_battleState.Value = BattleState.PlayerFaze;
        m_charactorManager.TurnBegin(m_currentTurn).Forget();
    }

    private void BattleEnd(BattleEndType battleEndType)
    {
        //ここで報酬表示
        switch (battleEndType)
        {
            case BattleEndType.EnemiesDead:
                //報酬のカードを抽選させる
                var cardDataList = m_cardDatas.GetData(m_charactorManager.CardClassType).GetCardDatas(m_rewardNum, m_currentBattleType, CardUpGrade.NoUpGrade);
                List<Card> cards = new List<Card>();
                cardDataList.ForEach(card =>
                {
                    Card c = Instantiate(m_cardPrefab);
                    c.Setup(card,
                        m_cardDatas.GetData(m_charactorManager.CardClassType).GetRaritySprite,
                        m_cardDatas.GetData(m_charactorManager.CardClassType).GetTypeSprite,
                        null);
                    //クリック時の振る舞い設定
                    c.OnClickSubject.Subscribe(_ =>
                    {
                        HaveCardData hcd = new HaveCardData();
                        hcd.Setup(m_charactorManager.CardClassType, card.ID, CardUpGrade.NoUpGrade);
                        m_charactorManager.HaveCard.Add(hcd);
                        m_battleState.Value = BattleState.None;
                    });
                    c.CardState = CardState.Button;
                    cards.Add(c);
                });
                //表示
                GameManager.Instance.CardDisplay(CardDisplayType.Reward, cards, () => m_battleFinished.OnNext(Unit.Default));
                //m_reward.ViewRewrard(objList);
                break;
            case BattleEndType.Gameover:
                Debug.Log("ゲームオーバー");
                return;
            default:
                break;
        }

        for (int i = m_currentCard.Count - 1; i >= 0; i--)
        {
            Destroy(m_currentCard[i].gameObject);
            m_currentCard.RemoveAt(i);
        }
        m_battleState.Value = BattleState.Reward;
    }

    public List<Card> GetCards(CardLotteryConditional conditional)
    {
        List<Card> ret = new List<Card>();
        switch (conditional)
        {
            case CardLotteryConditional.IsNoUpgrade:
                //未強化のカードだけ表示する
                for (int i = 0; i < m_charactorManager.HaveCard.Count; i++)
                {
                    if (m_charactorManager.HaveCard[i].IsUpGrade != CardUpGrade.NoUpGrade)
                        continue;
                    var db = m_cardDatas.GetDataBase(m_charactorManager.HaveCard[i]);
                    Card c = Instantiate(m_cardPrefab);
                    c.Setup(db,
                        m_cardDatas.GetData(m_charactorManager.CardClassType).GetRaritySprite,
                        m_cardDatas.GetData(m_charactorManager.CardClassType).GetTypeSprite,
                        null);
                    c.Index = i;
                    c.CardState = CardState.Button;
                    c.OnClickSubject.Subscribe(_ =>
                    {
                        Debug.Log($"index{c.Index}の{c.Name}を強化");
                        HaveCardData hcd = new HaveCardData();
                        hcd.Setup(m_charactorManager.HaveCard[c.Index].CardCalssType, 
                            m_charactorManager.HaveCard[c.Index].CardID, 
                            CardUpGrade.AsseptUpGrade);
                        m_charactorManager.HaveCard[c.Index] = hcd;
                    });
                    ret.Add(c);
                }
                break;
            default:
                break;
        }
        return ret;
    }
    #region テスト用関数
    /// <summary>報酬で出現するカードの確率空間が正しいかを調査する</summary>
    private void CardRewardTest(int num)
    {
        var cardIDs = System.Enum.GetNames(typeof(OriginalCardID));
        foreach (var c in cardIDs)
        {
            Debug.Log(c);
        }
        int[] ids = new int[cardIDs.Length];
        for (int i = 0; i < num; i++)
        {
            int id = m_cardDatas.GetData(CardClassType.Original).GetCardDatas(1, BattleType.Normal, CardUpGrade.NoUpGrade)[0].ID;
            ids[id]++;
        }
        for (int i = 0; i < ids.Length; i++)
        {
            Debug.Log($"ID:{(OriginalCardID)i}のカードが{ids[i]}枚抽選された");
        }
    }
    #endregion
}

public enum BattleState
{
    None,
    EnemyFaze,
    PlayerFaze,
    Reward,
}
public enum BattleType
{
    Normal,
    Elite,
    Boss,
}
/// <summary>
/// カードを抽選する際の条件
/// </summary>
public enum CardLotteryConditional
{
    None,
    IsNoUpgrade,
}