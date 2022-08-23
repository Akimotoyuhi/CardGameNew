using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;
using DG.Tweening;

/// <summary>
/// ゲーム中のGUIの操作や画面の切り替えをする
/// </summary>
public class GUIManager : MonoBehaviour
{
    [Header("共通")]
    /// <summary>マップ画面</summary>
    [SerializeField] GameObject m_mapPanel;
    /// <summary>全体情報を表示する画面</summary>
    [SerializeField] GameObject m_infoPanel;
    /// <summary>全体情報を表示するテキスト</summary>
    [SerializeField] Text m_infoText;
    /// <summary>カード一覧を表示する画面</summary>
    [SerializeField] GameObject m_displayPanel;
    /// <summary>カード一覧画面で表示するカードの親</summary>
    [SerializeField] Transform m_displayCardParent;
    /// <summary>フェード用パネル</summary>
    [SerializeField] Image m_fadeImage;
    [SerializeField, Header("ゲームオーバー画面")] GameoverScreen m_gameoverScreen;
    [Header("戦闘画面")]
    [SerializeField] GameObject m_battlePanel;
    [SerializeField] BattleManager m_battleManager;
    [SerializeField] CharactorManager m_charactorManager;
    [SerializeField] Button m_turnEndButton;
    [SerializeField] Text m_costText;
    [SerializeField] BattleAnimationUIController m_battleAnimationUIController;
    [Header("報酬画面")]
    [SerializeField] GameObject m_rewardPanel;
    [SerializeField] Transform m_rewardParent;
    [Header("マップ中画面")]
    [SerializeField] EventManager m_eventManager;
    /// <summary>休憩イベントを表示する画面</summary>
    [Header("休憩マス")]
    [SerializeField] GameObject m_restEventPanel;
    /// <summary>休むボタン</summary>
    [SerializeField] Button m_restButton;
    /// <summary>カード強化ボタン</summary>
    [SerializeField] Button m_upgradeButton;
    /// <summary>カード削除ボタン</summary>
    [SerializeField] Button m_cardClearButton;
    /// <summary>カード強化、削除の確認画面</summary>
    [SerializeField] GameObject m_checkPanel;
    /// <summary>カード強化確認画面で、強化前のカードの情報を表示する場所</summary>
    [SerializeField] Transform m_beforeCardParent;
    /// <summary>カード強化確認画面で、強化後のカードの情報を表示する場所</summary>
    [SerializeField] Transform m_afterCardParent;
    //カード削除確認画面で、カードの情報を表示する場所
    [SerializeField] Transform m_disposeParent;
    [SerializeField] Button m_applyButton;
    [SerializeField] Button m_calcelButton;
    /// <summary>フェード用シーケンス<br/>使わんかも</summary>
    private Sequence m_fadeSequence;
    private static Image FadeImage { get; set; }
    private static BattleAnimationUIController BattleAnimationUIController { get; set; }

    public void Setup()
    {
        //ターン終了ボタンが押されたらバトルマネージャーのターン終了関数を呼ぶ
        m_turnEndButton.onClick.AddListener(() => m_battleManager.OnBattle());

        //GameStateを監視して現在のStateに応じたパネルを表示する
        GameManager.Instance.GameStateObservable
            .Subscribe(s => SwitchGameState(s)).AddTo(this);

        //BattleStateを監視して現在のStateに応じたパネルを表示する
        m_battleManager.BattleStateObservable
            .Subscribe(s => SwitchBattleState(s)).AddTo(m_battleManager);

        //プレイヤーのコストを監視してコストのテキストを変更する
        m_charactorManager.CurrentPlayer.CurrentCostObservable
            .Subscribe(c => m_costText.text = $"{c}/{m_charactorManager.CurrentPlayer.MaxCost}")
            .AddTo(m_charactorManager.CurrentPlayer);

        //infoTextを監視してinfoTextの更新と表示非表示を切り替える
        GameManager.Instance.InfoTextUpdate
            .Subscribe(s => SetInfoTextPanels(s)).AddTo(this);
        m_infoPanel.SetActive(false);

        m_battleAnimationUIController.Setup();
        BattleAnimationUIController = m_battleAnimationUIController;

        //ゲームオーバーのイベントを受け取る
        m_battleManager.GameFinished
            .ThrottleFirst(System.TimeSpan.FromSeconds(m_gameoverScreen.ToGameoverScreenTime)) //連続攻撃等で複数回呼ばれないように
            .Subscribe(async type =>
                await Fade(Color.black, m_gameoverScreen.ToGameoverScreenTime, () =>
                {
                    Fade(Color.clear, 0).Forget();
                    m_gameoverScreen.SetActive(true, type);
                }))
            .AddTo(m_battleManager);

        //ゲームオーバー画面のセットアップ
        m_gameoverScreen.Setup(this);
        m_gameoverScreen.SetActive(false);

        //休憩マス関連
        {
            //休憩ボタン
            m_restButton.OnClickAsObservable()
                .ThrottleFirst(System.TimeSpan.FromSeconds(1))
                .Subscribe(_ =>
                {
                    m_eventManager.OnRest();
                    GameManager.Instance.FloorFinished();
                });

            //カード強化ボタン
            m_upgradeButton.OnClickAsObservable()
                .ThrottleFirst(System.TimeSpan.FromSeconds(0.1))
                .Subscribe(_ =>
                {
                    m_displayPanel.SetActive(true);
                    m_eventManager.EventType = EventType.Upgrade;
                    CardDisplay(CardDisplayType.List,
                        m_battleManager.GetCards(CardLotteryType.IsNoUpgrade),
                        () => m_checkPanel.SetActive(true));
                });

            //カード削除ボタン
            m_cardClearButton.OnClickAsObservable()
                .ThrottleFirst(System.TimeSpan.FromSeconds(0.1))
                .Subscribe(_ =>
                {
                    m_displayPanel.SetActive(true);
                    m_eventManager.EventType = EventType.Dispose;
                    CardDisplay(CardDisplayType.List,
                        m_battleManager.GetCards(CardLotteryType.Dispose),
                        () => m_checkPanel.SetActive(true));
                });

            //確認画面
            //確定ボタン
            m_applyButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    m_eventManager.IsDecision = true;
                    DisposeCardDisplay();
                    GameManager.Instance.FloorFinished();
                });

            //キャンセルボタン
            m_calcelButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    m_checkPanel.SetActive(false);
                    m_restEventPanel.SetActive(true);
                    m_eventManager.RetryRest();
                });
        }
        m_displayPanel.SetActive(false);
        FadeImage = m_fadeImage;
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
                //カード一覧画面を表示
                m_displayPanel.SetActive(true);
                foreach (var c in cards)
                {
                    c.transform.SetParent(m_displayCardParent, false);
                    c.OnClickSubject.Subscribe(_ =>
                    {
                        onClick();
                        CheckPanelPreview(m_charactorManager.HaveCard[c.Index], m_eventManager.EventType);
                        m_eventManager.SetSelectedCardIndex = c.Index;
                    }).AddTo(c);
                }
                break;
            case CardDisplayType.Reward:
                //報酬画面を表示
                foreach (var c in cards)
                {
                    c.transform.SetParent(m_rewardParent, false);
                    c.OnClickSubject.Subscribe(_ =>
                    {
                        onClick();
                        m_eventManager.SetSelectedCardIndex = c.Index;
                        DisposeCardDisplay();
                    }).AddTo(c);
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
    private void DisposeCardDisplay()
    {
        for (int i = m_displayCardParent.childCount - 1; i >= 0; i--)
        {
            Destroy(m_displayCardParent.GetChild(i).gameObject);
        }
        for (int i = m_rewardParent.childCount - 1; i >= 0; i--)
        {
            Destroy(m_rewardParent.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// 確認画面の設定
    /// </summary>
    /// <param name="haveCardData"></param>
    /// <param name="eventType"></param>
    private void CheckPanelPreview(HaveCardData haveCardData, EventType eventType)
    {
        //画面の初期化
        if (m_beforeCardParent.childCount > 0)
            Destroy(m_beforeCardParent.GetChild(0).gameObject);
        if (m_afterCardParent.childCount > 0)
            Destroy(m_afterCardParent.GetChild(0).gameObject);
        if (m_disposeParent.childCount > 0)
            Destroy(m_disposeParent.GetChild(0).gameObject);

        switch (eventType)
        {
            case EventType.Upgrade:
                //カード強化の確認画面　強化前と後のカードを表示させる
                Card beforeCard = m_battleManager.GetCardInstance(haveCardData, CardState.None);
                beforeCard.transform.SetParent(m_beforeCardParent, false);
                haveCardData.IsUpGrade = CardUpGrade.AsseptUpGrade;
                Card Aftercard = m_battleManager.GetCardInstance(haveCardData, CardState.None);
                Aftercard.transform.SetParent(m_afterCardParent, false);
                break;
            case EventType.Dispose:
                Card disposeCard = m_battleManager.GetCardInstance(haveCardData, CardState.None);
                disposeCard.transform.SetParent(m_disposeParent, false);
                break;
            default:
                break;
        }
    }

    /// <summary>GameStateに応じて画面を切り替える</summary>
    private void SwitchGameState(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.MapSelect:
                m_mapPanel.SetActive(true);
                m_battlePanel.SetActive(false);
                m_restEventPanel.SetActive(false);
                m_displayPanel.SetActive(false);
                m_checkPanel.SetActive(false);
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

    /// <summary>BattleStateに応じて画面を切り替える</summary>
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

    /// <summary>
    /// フェード
    /// </summary>
    /// <param name="color"></param>
    /// <param name="duration"></param>
    /// <param name="onCompleate"></param>
    /// <returns></returns>
    public static async UniTask Fade(Color color, float duration, System.Action onCompleate = null)
    {
        if (color != Color.clear)
            FadeImage.raycastTarget = true;
        else
            FadeImage.raycastTarget = false;
        await FadeImage.DOColor(color, duration).OnComplete(() =>
        {
            if (onCompleate != null)
                onCompleate();
        });
    }

    /// <summary>
    /// 戦闘中のテキストアニメーションの再生
    /// </summary>
    /// <param name="animationType"></param>
    /// <returns></returns>
    public static async UniTask PlayBattleUIAnimation(BattleAnimationUIMoveTextType animationType) => 
        await BattleAnimationUIController.ActiveText(animationType);
}

public enum CardDisplayType
{
    List,
    Reward,
}