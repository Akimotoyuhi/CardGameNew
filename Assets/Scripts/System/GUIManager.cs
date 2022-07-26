using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;
using DG.Tweening;

/// <summary>
/// �Q�[������GUI�̑�����ʂ̐؂�ւ�������
/// </summary>
public class GUIManager : MonoBehaviour
{
    [Header("����")]
    /// <summary>�}�b�v���</summary>
    [SerializeField] GameObject m_mapPanel;
    /// <summary>�S�̏���\��������</summary>
    [SerializeField] GameObject m_infoPanel;
    /// <summary>�S�̏���\������e�L�X�g</summary>
    [SerializeField] Text m_infoText;
    /// <summary>�J�[�h�ꗗ��\��������</summary>
    [SerializeField] GameObject m_displayPanel;
    /// <summary>�J�[�h�ꗗ��ʂŕ\������J�[�h�̐e</summary>
    [SerializeField] Transform m_displayCardParent;
    //�t�F�[�h�p�p�l��
    [SerializeField] Image m_fadeImage;
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
    /// <summary>�x�e�C�x���g��\��������</summary>
    [Header("�x�e�}�X")]
    [SerializeField] GameObject m_restEventPanel;
    /// <summary>�x�ރ{�^��</summary>
    [SerializeField] Button m_restButton;
    /// <summary>�J�[�h�����{�^��</summary>
    [SerializeField] Button m_upgradeButton;
    /// <summary>�J�[�h�폜�{�^��</summary>
    [SerializeField] Button m_cardClearButton;
    /// <summary>�J�[�h�����A�폜�̊m�F���</summary>
    [SerializeField] GameObject m_checkPanel;
    [SerializeField] Transform m_beforeCardParent;
    [SerializeField] Transform m_afterCardParent;
    [SerializeField] Button m_applyButton;
    [SerializeField] Button m_calcelButton;
    /// <summary>�t�F�[�h�p�V�[�P���X</summary>
    private Sequence m_fadeSequence;
    private EventType m_type;
    private static Image FadeImage { get; set; }

    public void Setup()
    {
        //�^�[���I���{�^���������ꂽ��o�g���}�l�[�W���[�̃^�[���I���֐����Ă�
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
                m_eventManager.EventType = EventType.Upgrade;
                CardDisplay(CardDisplayType.List,
                    m_battleManager.GetCards(CardLotteryType.IsNoUpgrade),
                    () => m_checkPanel.SetActive(true));
            });

            //�J�[�h�폜�{�^��
            m_cardClearButton.onClick.AddListener(() =>
            {
                m_displayPanel.SetActive(true);
                m_eventManager.EventType = EventType.Dispose;
                CardDisplay(CardDisplayType.List,
                    m_battleManager.GetCards(CardLotteryType.Dispose),
                    () => m_checkPanel.SetActive(true));
            });

            //�m�F���
            //�m��{�^��
            m_applyButton.onClick.AddListener(() =>
            {
                m_eventManager.IsDecision = true;
                DisposeCardDisplay();
                GameManager.Instance.FloorFinished();
            });

            //�L�����Z���{�^��
            m_calcelButton.onClick.AddListener(() =>
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
                //�J�[�h�ꗗ��ʂ�\��
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
                //��V��ʂ�\��
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
    /// �J�[�h�ꗗ��ʂ̃��Z�b�g
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
    /// �m�F��ʂ̐ݒ�
    /// </summary>
    /// <param name="haveCardData"></param>
    /// <param name="eventType"></param>
    private void CheckPanelPreview(HaveCardData haveCardData, EventType eventType)
    {
        switch (eventType)
        {
            case EventType.Upgrade:
                Card beforeCard = m_battleManager.GetCardInstance(haveCardData, CardState.None);
                beforeCard.transform.SetParent(m_beforeCardParent, false);
                haveCardData.IsUpGrade = CardUpGrade.AsseptUpGrade;
                Card Aftercard = m_battleManager.GetCardInstance(haveCardData, CardState.None);
                Aftercard.transform.SetParent(m_afterCardParent, false);
                break;
            case EventType.Dispose:
                break;
            default:
                break;
        }
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

    /// <summary>
    /// �t�F�[�h
    /// </summary>
    /// <param name="color"></param>
    /// <param name="duration"></param>
    /// <param name="onCompleate"></param>
    public static void Fade(Color color, float duration, System.Action onCompleate = null)
    {
        if (color != Color.clear)
            FadeImage.raycastTarget = true;
        else
            FadeImage.raycastTarget = false;
        FadeImage.DOColor(color, duration).OnComplete(() =>
        {
            if (onCompleate != null)
                onCompleate();
        });
    }

    /// <summary>
    /// �t�F�[�h(await�o����)
    /// </summary>
    /// <param name="color"></param>
    /// <param name="duration"></param>
    /// <param name="onCompleate"></param>
    /// <returns></returns>
    public static async UniTask FadeAsync(Color color, float duration, System.Action onCompleate = null)
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
}

public enum CardDisplayType
{
    List,
    Reward,
}