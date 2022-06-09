using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

public class BattleManager : MonoBehaviour
{
    [SerializeField] CardData m_commonCardData;
    [SerializeField] CardData m_originalCardData;
    [SerializeField] CardData m_akCardData;
    [SerializeField] Card m_cardPrefab;
    [SerializeField] Transform m_hand;
    [SerializeField] CharactorManager m_charactorManager;
    private List<Card> m_currentCard = new List<Card>();
    private CardData m_useCardData;
    public CardData CardData => m_akCardData;

    public void Setup()
    {
        m_charactorManager.Setup();
        Create();
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
        list.ForEach(i =>
        {
            Card c = Instantiate(m_cardPrefab);
            c.Setup(m_akCardData.DataBases[i], m_charactorManager.CurrentPlayer);//とりあえず
            c.CardUsed.Subscribe(cmds => CommandExecutor(cmds)).AddTo(this);
            c.transform.SetParent(m_hand, false);
        });
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
