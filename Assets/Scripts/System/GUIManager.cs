using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;

/// <summary>
/// ゲーム中のGUIの操作や画面の切り替えをする
/// </summary>
public class GUIManager : MonoBehaviour
{
    [SerializeField] GameObject m_mapPanel;
    [SerializeField] GameObject m_infoPanel;
    [SerializeField] Text m_infoText;
    [SerializeField] GameObject m_displayPanel;
    [SerializeField] Transform m_uiViewParent;
    [Header("戦闘画面")]
    [SerializeField] GameObject m_battlePanel;
    [SerializeField] BattleManager m_battleManager;
    [SerializeField] CharactorManager m_charactorManager;
    [SerializeField] Button m_turnEndButton;
    [SerializeField] Text m_costText;
    [Header("報酬画面")]
    [SerializeField] GameObject m_rewardPanel;
    [SerializeField] Transform m_rewardParent;
    [Header("マップ中画面")]
    [SerializeField] MapEvent m_mapEvant;
    [SerializeField] GameObject m_restEventPanel;
    [SerializeField] Button m_restButton;
    [SerializeField] Button m_upgradeButton;
    [SerializeField] Button m_cardClearButton;

    public void Setup()
    {
        //ターン終了ボタンが押されたらバトルマネージャーのターン終了関数を押す
        m_turnEndButton.onClick.AddListener(() => m_battleManager.OnBattle());
        
        //GameStateを監視して現在のStateに応じたパネルを表示する
        GameManager.Instance.GameStateObservable
            .Subscribe(s => SwitchGameState(s)).AddTo(this);
        
        //BattleStateを監視して現在のStateに応じたパネルを表示する
        m_battleManager.BattleStateObservable
            .Subscribe(s => SwitchBattleState(s)).AddTo(m_battleManager);

        //プレイヤーのコストを監視してコストのテキストを変更する
        m_charactorManager.CurrentPlayer.CurrentCostObservable
            .Subscribe(c => m_costText.text = $"{c}/{m_charactorManager.CurrentPlayer.MaxCost}").AddTo(m_charactorManager.CurrentPlayer);
        
        //infoTextを監視してinfoTextの更新と表示非表示を切り替える
        GameManager.Instance.InfoTextUpdate
            .Subscribe(s => SetInfoTextPanels(s)).AddTo(this);
        m_infoPanel.SetActive(false);

        //休憩マス関連
        {
            //休憩ボタン
            m_restButton.onClick.AddListener(() =>
            {
                m_charactorManager.CurrentPlayer.HealEvent(m_mapEvant.RestEvent.Heal);
            });

            //カード強化ボタン
            m_upgradeButton.onClick.AddListener(() =>
            {

            });

            //カード削除ボタン
            m_cardClearButton.onClick.AddListener(() =>
            {

            });
        }
    }

    /// <summary>
    /// カードの一覧を表示する
    /// </summary>
    /// <param name="displayType">表示タイプ</param>
    /// <param name="cards">表示するカード</param>
    /// <param name="onClick">カードをクリックした時の振る舞い</param>
    public void CardDisplay(CardDisplayType displayType, List<Card> cards, System.Action onClick)
    {
        switch (displayType)
        {
            case CardDisplayType.List:
                m_displayPanel.SetActive(true);
                foreach (var c in cards)
                {
                    c.transform.SetParent(m_uiViewParent, false);
                    c.OnClickSubject.Subscribe(_ =>
                    {
                        onClick();
                        DisposeCardDisplay(displayType);
                    });
                }
                break;
            case CardDisplayType.Reward:
                //m_rewardPanel.SetActive(true);
                foreach (var c in cards)
                {
                    c.transform.SetParent(m_rewardParent, false);
                    c.OnClickSubject.Subscribe(_ =>
                    {
                        onClick();
                        DisposeCardDisplay(displayType);
                    });
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// カード一覧画面のリセット
    /// </summary>
    /// <param name="displayType"></param>
    private void DisposeCardDisplay(CardDisplayType displayType)
    {
        switch (displayType)
        {
            case CardDisplayType.List:
                for (int i = m_uiViewParent.childCount - 1; i >= 0; i--)
                {
                    Destroy(m_uiViewParent.GetChild(i).gameObject);
                }
                m_displayPanel.SetActive(false);
                break;
            case CardDisplayType.Reward:
                for (int i = m_rewardParent.childCount - 1; i >= 0; i--)
                {
                    Destroy(m_rewardParent.GetChild(i).gameObject);
                }
                break;
            default:
                break;
        }
    }

    /// <summary>GameStateに応じてUIを切り替える</summary>
    private void SwitchGameState(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.MapSelect:
                m_mapPanel.SetActive(true);
                m_battlePanel.SetActive(false);
                m_restEventPanel.SetActive(false);
                break;
            case GameState.Battle:
                m_mapPanel.SetActive(false);
                m_battlePanel.SetActive(true);
                m_restEventPanel.SetActive(false);
                break;
            case GameState.Rest:
                m_mapPanel.SetActive(false);
                m_battlePanel.SetActive(false);
                m_restEventPanel.SetActive(true);
                break;
            default:
                break;
        }
    }

    /// <summary>BattleStateに応じてUIを切り替える</summary>
    /// <param name="battleState"></param>
    private void SwitchBattleState(BattleState battleState)
    {
        switch (battleState)
        {
            case BattleState.None:
                m_turnEndButton.interactable = false;
                m_rewardPanel.SetActive(false);
                break;
            case BattleState.EnemyFaze:
                m_turnEndButton.interactable = false;
                m_rewardPanel.SetActive(false);
                break;
            case BattleState.PlayerFaze:
                m_turnEndButton.interactable = true;
                m_rewardPanel.SetActive(false);
                break;
            case BattleState.Reward:
                m_turnEndButton.interactable = false;
                m_rewardPanel.SetActive(true);
                break;
            default:
                break;
        }
    }

    private void SetInfoTextPanels(string text)
    {
        m_infoText.text = text;
        if (text == "")
            m_infoPanel.SetActive(false);
        else
            m_infoPanel.SetActive(true);
    }
}

public enum CardDisplayType
{
    List,
    Reward,
}