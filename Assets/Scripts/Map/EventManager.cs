using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;

public class EventManager : MonoBehaviour
{
    [SerializeField] Rest m_rest;
    [SerializeField] CharactorManager m_charactorManager;
    private EventType m_eventType;
    private ReactiveProperty<MapEventState> m_mapState = new ReactiveProperty<MapEventState>();
    //private ReactiveProperty<int> m_selectedCardIndex = new ReactiveProperty<int>();
    private int m_selectedCardIndex;
    private CancellationTokenSource m_cancellationTokenSource;
    public EventType SetEventType { set => m_eventType = value; }
    /// <summary>イベントの状態</summary>
    public System.IObservable<MapEventState> MapEventStateObservable => m_mapState;
    /// <summary>選択されたカードのindex</summary>
    public int SetSelectedCardIndex { set => m_selectedCardIndex = value; }

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
        m_selectedCardIndex = -1;
        await UniTask.WaitUntil(() => m_selectedCardIndex != -1, 
            PlayerLoopTiming.Update, 
            m_cancellationTokenSource.Token);
        switch (m_eventType)
        {
            case EventType.Upgrade:
                Debug.Log($"Index{m_selectedCardIndex}番のカードを強化");
                break;
            case EventType.Dispose:
                Debug.Log($"Index{m_selectedCardIndex}番のカードを削除");
                break;
            default:
                break;
        }
    }

    public void OnRest()
    {
        m_charactorManager.CurrentPlayer.HealEvent(m_rest.Heal);
        m_cancellationTokenSource.Cancel();
    }

    public void OnUpgrade()
    {

    }

    public void OnDispose()
    {

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