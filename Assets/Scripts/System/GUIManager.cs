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
    /// <summary>�}�b�v���</summary>
    [SerializeField] GameObject m_mapPanel;
    /// <summary>�S�̏���\��������</summary>
    [SerializeField] GameObject m_infoPanel;
    [SerializeField] Text m_infoText;
    /// <summary>�J�[�h�ꗗ��\��������</summary>
    [SerializeField] GameObject m_displayPanel;
    [SerializeField] Transform m_uiViewParent;
    [Header("�퓬���")]
    [SerializeField] GameObject m_battlePanel;
    [SerializeField] BattleManager m_battleManager;
    [SerializeField] CharactorManager m_charactorManager;
    [SerializeField] Button m_turnEndButton;
    [SerializeField] Text m_costText;
    [Header("��V���")]
    [SerializeField] GameObject m_rewardPanel;
    [SerializeField] Transform m_rewardParent;
    [Header("�}�b�v�����")]
    [SerializeField] EventManager m_eventManager;
    [Header("�x�e�}�X")]
    [SerializeField] GameObject m_restEventPanel;
    [SerializeField] Button m_restButton;
    [SerializeField] Button m_upgradeButton;
    [SerializeField] Button m_cardClearButton;
    [SerializeField] GameObject m_checkPanel;
    [SerializeField] Transform m_beforeCardParent;
    [SerializeField] Transform m_aftarCardParent;
    [SerializeField] Button m_applyButton;
    [SerializeField] Button m_calcelButton;

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
            .Subscribe(c => m_costText.text = $"{c}/{m_charactorManager.CurrentPlayer.MaxCost}")
            .AddTo(m_charactorManager.CurrentPlayer);
        
        //infoText���Ď�����infoText�̍X�V�ƕ\����\����؂�ւ���
        GameManager.Instance.InfoTextUpdate
            .Subscribe(s => SetInfoTextPanels(s)).AddTo(this);
        m_infoPanel.SetActive(false);

        //�x�e�}�X�֘A
        {
            //�x�e�{�^��
            m_restButton.onClick.AddListener(() =>
            {
                m_eventManager.OnRest();
                GameManager.Instance.FloorFinished();
            });

            //�J�[�h�����{�^��
            m_upgradeButton.onClick.AddListener(() =>
            {
                m_displayPanel.SetActive(true);
                m_eventManager.SetEventType = EventType.Upgrade;
                CardDisplay(CardDisplayType.List,
                    m_battleManager.GetCards(CardLotteryType.IsNoUpgrade),
                    () => Debug.Log("a"));
            });

            //�J�[�h�폜�{�^��
            m_cardClearButton.onClick.AddListener(() =>
            {
                m_displayPanel.SetActive(true);
                m_eventManager.SetEventType = EventType.Dispose;
                CardDisplay(CardDisplayType.List,
                    m_battleManager.GetCards(CardLotteryType.Dispose),
                    () => Debug.Log("b"));
            });

            //�m�F���
            //�m��{�^��
            m_applyButton.onClick.AddListener(() =>
            {
                DisposeCardDisplay();
                GameManager.Instance.FloorFinished();
            });

            //�L�����Z���{�^��
            m_calcelButton.onClick.AddListener(() =>
            {
                m_restEventPanel.SetActive(true);
            });
        }
        m_displayPanel.SetActive(false);
    }

    /// <summary>
    /// �J�[�h�̈ꗗ��\������
    /// </summary>
    /// <param name="displayType">�\���^�C�v</param>
    /// <param name="cards">�\������J�[�h</param>
    /// <param name="onClick">�J�[�h���N���b�N�������̐U�镑��</param>
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
                        m_eventManager.SetSelectedCardIndex = c.Index;
                        DisposeCardDisplay(displayType);//���Ƃł���
                    }).AddTo(c);
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
                        m_eventManager.SetSelectedCardIndex = c.Index;
                        DisposeCardDisplay(displayType);//���Ƃł���
                    }).AddTo(c);
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// �J�[�h�ꗗ��ʂ̃��Z�b�g
    /// </summary>
    /// <param name="displayType"></param>
    private void DisposeCardDisplay(CardDisplayType displayType = CardDisplayType.List)
    {
        for (int i = m_uiViewParent.childCount - 1; i >= 0; i--)
        {
            Destroy(m_uiViewParent.GetChild(i).gameObject);
        }
        for (int i = m_rewardParent.childCount - 1; i >= 0; i--)
        {
            Destroy(m_rewardParent.GetChild(i).gameObject);
        }
        /*
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
        */
    }

    /// <summary>GameState�ɉ����ĉ�ʂ�؂�ւ���</summary>
    private void SwitchGameState(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.MapSelect:
                m_mapPanel.SetActive(true);
                m_battlePanel.SetActive(false);
                m_restEventPanel.SetActive(false);
                m_displayPanel.SetActive(false);
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

    /// <summary>BattleState�ɉ����ĉ�ʂ�؂�ւ���</summary>
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