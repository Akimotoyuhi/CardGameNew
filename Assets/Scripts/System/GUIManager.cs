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
    [SerializeField] GameObject m_rewardPanel;
    [Header("�}�b�v�����")]
    [SerializeField] MapEvent m_mapEvant;
    [SerializeField] GameObject m_restEventPanel;
    [SerializeField] Button m_restButton;
    [SerializeField] Button m_upgradeButton;
    [SerializeField] Button m_cardClearButton;

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

        //�x�e�}�X�֘A
        {
            //�x�e�{�^��
            m_restButton.onClick.AddListener(() =>
            {
                m_charactorManager.CurrentPlayer.HealEvent(m_mapEvant.RestEvent.Heal);
            });

            //�J�[�h�����{�^��
            m_upgradeButton.onClick.AddListener(() =>
            {

            });

            //�J�[�h�폜�{�^��
            m_cardClearButton.onClick.AddListener(() =>
            {

            });
        }
    }

    /// <summary>GameState�ɉ�����UI��؂�ւ���</summary>
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

    /// <summary>BattleState�ɉ�����UI��؂�ւ���</summary>
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
