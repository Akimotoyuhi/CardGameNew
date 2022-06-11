using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;

public class GUIManager : MonoBehaviour
{
    [Header("í“¬‰æ–Ê")]
    [SerializeField] BattleManager m_battleManager;
    [SerializeField] CharactorManager m_charactorManager;
    [SerializeField] Button m_turnEndButton;
    [SerializeField] Text m_costText;

    public void Setup()
    {
        m_turnEndButton.onClick.AddListener(() => m_battleManager.OnBattle());
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
