using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;

/// <summary>
/// �Q�[������GUI�̑�����ʂ̐؂�ւ�������
/// </summary>
public class GUIManager : MonoBehaviour
{
    [SerializeField] GameObject m_mapPanel;
    [SerializeField] GameObject m_infoPanel;
    [SerializeField] Text m_infoText;
    [Header("�퓬���")]
    [SerializeField] GameObject m_battlePanel;
    [SerializeField] BattleManager m_battleManager;
    [SerializeField] CharactorManager m_charactorManager;
    [SerializeField] Button m_turnEndButton;
    [SerializeField] Text m_costText;
    public void Setup()
    {
        //�^�[���I���{�^���������ꂽ��o�g���}�l�[�W���[�̃^�[���I���֐�������
        m_turnEndButton.onClick.AddListener(() => m_battleManager.OnBattle());
        
        //GameState���Ď����Č��݂�State�ɉ������p�l����\������
        GameManager.Instance.GameStateObservable
            .Subscribe(s => SwitchGameState(s)).AddTo(this);
        
        //BattleState���Ď����Č��݂�State�ɉ������p�l����\������
        m_battleManager.BattleStateObservable
            .Subscribe(s => SwitchBattleState(s)).AddTo(m_battleManager);

        //�v���C���[�̃R�X�g���Ď����ăR�X�g�̃e�L�X�g��ύX����
        m_charactorManager.CurrentPlayer.CurrentCostObservable
            .Subscribe(c => m_costText.text = $"{c}/{m_charactorManager.CurrentPlayer.MaxCost}").AddTo(m_charactorManager.CurrentPlayer);
        
        //infoText���Ď�����infoText�̍X�V�ƕ\����\����؂�ւ���
        GameManager.Instance.InfoTextUpdate
            .Subscribe(s => SetInfoTextPanels(s)).AddTo(this);
        m_infoPanel.SetActive(false);
    }

    /// <summary>GameState�ɉ�����UI��؂�ւ���</summary>
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

    /// <summary>BattleState�ɉ�����UI��؂�ւ���</summary>
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
