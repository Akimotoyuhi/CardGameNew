using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class GameManager : MonoBehaviour
{
    [SerializeField] BattleManager m_battleManager;
    [SerializeField] EventManager m_eventManager;
    [SerializeField] GUIManager m_guiManager;
    [SerializeField] MapManager m_mapManager;
    private ReactiveProperty<GameState> m_gameState = new ReactiveProperty<GameState>();
    private ReactiveProperty<string> m_infoText = new ReactiveProperty<string>();
    private ReactiveProperty<int> m_floor = new ReactiveProperty<int>();
    public static GameManager Instance { get; private set; }
    /// <summary>GameStateが変更された事を通知する</summary>
    public System.IObservable<GameState> GameStateObservable => m_gameState;
    /// <summary>インフォメーションテキストの更新用</summary>
    public string SetInfoText { set => m_infoText.Value = value; }
    /// <summary>インフォメーションテキストが更新されたら通知する</summary>
    public System.IObservable<string> InfoTextUpdate => m_infoText;
    /// <summary>現在の階層が更新されたら通知する</summary>
    public System.IObservable<int> FloorUpdate => m_floor;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //バトルマネージャーのセットアップ
        m_battleManager.Setup();
        m_battleManager.BattleFinished
            .Subscribe(_ => FloorFinished()).AddTo(m_battleManager);

        //GUIマネージャーのセットアップ
        m_guiManager.Setup();

        //マップマネージャーのセットアップ
        m_mapManager.Setup();
        m_mapManager.EncountObservable.Subscribe(ct => Encount(ct)).AddTo(m_mapManager);

        //イベントマネージャーのセットアップ
        m_eventManager.Setup();

        //最初はマップセレクト画面から始まる
        m_gameState.Value = GameState.MapSelect;
        m_floor.Value = 1;
    }

    /// <summary>
    /// 押されたマップのマスに応じて何かしらの処理をする
    /// </summary>
    /// <param name="cellType"></param>
    private async void Encount(CellType cellType)
    {
        await m_guiManager.FadeAsync(Color.black, 0.5f, () => m_guiManager.Fade(Color.clear, 0.5f));
        switch (cellType)
        {
            case CellType.Rest:
                m_gameState.Value = GameState.Rest;
                break;
            default:
                //戦闘マスだった場合
                m_gameState.Value = GameState.Battle;
                m_battleManager.Encount(m_mapManager.NowMapID, cellType);
                break;
        }
    }

    /// <summary>
    /// 階層の更新
    /// </summary>
    public void FloorFinished()
    {
        m_floor.Value++;
        Debug.Log($"現在フロア {m_floor.Value}");
        m_gameState.Value = GameState.MapSelect;
    }

    /// <summary>
    /// カード一覧を表示する
    /// </summary>
    /// <param name="displayType">表示する形式</param>
    /// <param name="cards">表示するカード</param>
    /// <param name="onClick">カードをクリックした際の振る舞い</param>
    public void CardDisplay(CardDisplayType displayType, List<Card> cards, System.Action onClick) => 
        m_guiManager.CardDisplay(displayType, cards, onClick);
}

public enum GameState
{
    MapSelect,
    Battle,
    Rest,
}