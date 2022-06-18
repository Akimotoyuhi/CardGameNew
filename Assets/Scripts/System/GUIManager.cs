using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;

public class GUIManager : MonoBehaviour
{
    [SerializeField] GameObject m_mapPanel;
    [Header("í“¬‰æ–Ê")]
    [SerializeField] GameObject m_battlePanel;
    [SerializeField] BattleManager m_battleManager;
    [SerializeField] CharactorManager m_charactorManager;
    [SerializeField] Button m_turnEndButton;
    [SerializeField] Text m_costText;

    public void Setup()
    {
        m_turnEndButton.onClick.AddListener(() => m_battleManager.OnBattle());
        GameManager.Instance.GameStateObservable
            .Subscribe(s =>
            {
                switch (s)
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
            })
            .AddTo(this);
        m_battleManager.BattleStateObservable
            .Subscribe(s =>
            {
                if (s == BattleState.PlayerFaze)
                    m_turnEndButton.interactable = true;
                else
                    m_turnEndButton.interactable = false;
            })
            .AddTo(m_battleManager);
        m_charactorManager.CurrentPlayer.CurrentCostObservable
            .Subscribe(c =>
            {
                m_costText.text = $"{c}/{m_charactorManager.CurrentPlayer.MaxCost}";
            })
            .AddTo(m_charactorManager.CurrentPlayer);
    }
}
