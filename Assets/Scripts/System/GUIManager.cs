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
    [Header("戦闘画面")]
    [SerializeField] GameObject m_battlePanel;
    [SerializeField] BattleManager m_battleManager;
    [SerializeField] CharactorManager m_charactorManager;
    [SerializeField] Button m_turnEndButton;
    [SerializeField] Text m_costText;
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

    /// <summary>GameStateに応じてUIを切り替える</summary>
    private void SwitchGameState(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.MapSelect:
                m_mapPanel.SetActive(true);
                m_battlePanel.SetActive(false);
                break;
            case GameState.Battle:
                m_mapPanel.SetActive(false);
                m_battlePanel.SetActive(true);
                break;
            default:
                break;
        }
    }

    /// <summary>BattleStateに応じてUIを切り替える</summary>
    /// <param name="battleState"></param>
    private void SwitchBattleState(BattleState battleState)
    {
        if (battleState == BattleState.PlayerFaze)
            m_turnEndButton.interactable = true;
        else
            m_turnEndButton.interactable = false;
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
