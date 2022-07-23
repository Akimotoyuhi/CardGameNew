using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;

public class EventManager : MonoBehaviour
{
    [SerializeField] CharactorManager m_charactorManager;
    [SerializeField] Rest m_rest;
    private EventType m_eventType;
    private ReactiveProperty<MapEventState> m_mapState = new ReactiveProperty<MapEventState>();
    private CancellationTokenSource m_cancellationTokenSource;
    public EventType SetEventType { set => m_eventType = value; }
    /// <summary>�C�x���g�̏��</summary>
    public System.IObservable<MapEventState> MapEventStateObservable => m_mapState;
    /// <summary>�I�����ꂽ�J�[�h��index</summary>
    public int SetSelectedCardIndex { private get; set; }
    /// <summary>�m�F��ʂ̑����҂p</summary>
    public bool IsDecision { private get; set; }

    public void Setup()
    {
        GameManager.Instance.GameStateObservable.Subscribe(s =>
        {
            switch (s)
            {
                case GameState.Rest:
                    RestEvent().Forget();
                    break;
                default:
                    break;
            }
        }).AddTo(this);
    }

    private async UniTask RestEvent()
    {
        m_cancellationTokenSource = new CancellationTokenSource();
        Debug.Log("�x�e�}�X�ɓ�����");
        SetSelectedCardIndex = -1;
        //�����J�[�h���I�������܂ő҂�
        await UniTask.WaitUntil(() => SetSelectedCardIndex != -1, 
            PlayerLoopTiming.Update, 
            m_cancellationTokenSource.Token);

        //�m��{�^�����������܂ő҂�
        Debug.Log("�m�F��ʂ̑����ҋ@");
        IsDecision = false;
        await UniTask.WaitUntil(() => IsDecision, 
            PlayerLoopTiming.Update, 
            m_cancellationTokenSource.Token);

        switch (m_eventType)
        {
            case EventType.Upgrade:
                HaveCardData hcd = new HaveCardData();
                hcd.Setup(m_charactorManager.HaveCard[SetSelectedCardIndex].CardCalssType,
                    m_charactorManager.HaveCard[SetSelectedCardIndex].CardID,
                    CardUpGrade.AsseptUpGrade);
                m_charactorManager.HaveCard[SetSelectedCardIndex] = hcd;
                Debug.Log($"Index{SetSelectedCardIndex}�Ԃ̃J�[�h������");
                break;
            case EventType.Dispose:
                m_charactorManager.HaveCard.RemoveAt(SetSelectedCardIndex);
                Debug.Log($"Index{SetSelectedCardIndex}�Ԃ̃J�[�h���폜");
                break;
            default:
                break;
        }
    }

    public void OnRest()
    {
        m_charactorManager.CurrentPlayer.HealEvent(m_rest.Heal);
        Debug.Log($"�v���C���[�̗̑͂�{m_rest.Heal}��");
        CancelRest();
    }

    /// <summary>
    /// �x�e�C�x���g�̃L�����Z��
    /// </summary>
    public void CancelRest()
    {
        m_cancellationTokenSource.Cancel();
        Debug.Log("RestEvent���L�����Z�����ꂽ");
    }

    public void RetryRest()
    {
        CancelRest();
        RestEvent().Forget();
    }





    [System.Serializable]
    public class Rest
    {
        [SerializeField] int m_healValue;
        [SerializeField] int m_upgradeNum;
        [SerializeField] int m_clearCardNum;
        /// <summary>�񕜗�</summary>
        public int Heal => m_healValue;
        /// <summary>��������</summary>
        public int UpgradeNum => m_upgradeNum;
        /// <summary>�폜����</summary>
        public int ClearNum => m_clearCardNum;
    }
}

/// <summary>
/// GUIManager����ǂ̋x�e�}�X�C�x���g�������������������Ă��炤�p
/// </summary>
public enum EventType
{
    Rest,
    Upgrade,
    Dispose,
}

/// <summary>
/// EventManager�̏��
/// </summary>
public enum MapEventState
{
    Upgrade,
    Dispose,
}