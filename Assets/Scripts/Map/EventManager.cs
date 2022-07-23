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
    /// <summary>イベントの状態</summary>
    public System.IObservable<MapEventState> MapEventStateObservable => m_mapState;
    /// <summary>選択されたカードのindex</summary>
    public int SetSelectedCardIndex { private get; set; }
    /// <summary>確認画面の操作を待つ用</summary>
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
        Debug.Log("休憩マスに入った");
        SetSelectedCardIndex = -1;
        //何かカードが選択されるまで待つ
        await UniTask.WaitUntil(() => SetSelectedCardIndex != -1, 
            PlayerLoopTiming.Update, 
            m_cancellationTokenSource.Token);

        //確定ボタンが押されるまで待つ
        Debug.Log("確認画面の操作を待機");
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
                Debug.Log($"Index{SetSelectedCardIndex}番のカードを強化");
                break;
            case EventType.Dispose:
                m_charactorManager.HaveCard.RemoveAt(SetSelectedCardIndex);
                Debug.Log($"Index{SetSelectedCardIndex}番のカードを削除");
                break;
            default:
                break;
        }
    }

    public void OnRest()
    {
        m_charactorManager.CurrentPlayer.HealEvent(m_rest.Heal);
        Debug.Log($"プレイヤーの体力を{m_rest.Heal}回復");
        CancelRest();
    }

    /// <summary>
    /// 休憩イベントのキャンセル
    /// </summary>
    public void CancelRest()
    {
        m_cancellationTokenSource.Cancel();
        Debug.Log("RestEventがキャンセルされた");
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
        /// <summary>回復量</summary>
        public int Heal => m_healValue;
        /// <summary>強化枚数</summary>
        public int UpgradeNum => m_upgradeNum;
        /// <summary>削除枚数</summary>
        public int ClearNum => m_clearCardNum;
    }
}

/// <summary>
/// GUIManagerからどの休憩マスイベントが発生したかを教えてもらう用
/// </summary>
public enum EventType
{
    Rest,
    Upgrade,
    Dispose,
}

/// <summary>
/// EventManagerの状態
/// </summary>
public enum MapEventState
{
    Upgrade,
    Dispose,
}