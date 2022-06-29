using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class GameManager : MonoBehaviour
{
    [SerializeField] BattleManager m_battleManager;
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

        //最初はマップセレクト画面から始まる
        m_gameState.Value = GameState.MapSelect;
        m_floor.Value = 1;
    }

    /// <summary>
    /// 押されたマップのマスに応じて何かしらの処理をする
    /// </summary>
    /// <param name="cellType"></param>
    private void Encount(CellType cellType)
    {
        if (cellType == CellType.Rest)
        {
            Debug.Log("休憩マス未実装");
            return;
        }

        //戦闘マスだった場合
        m_gameState.Value = GameState.Battle;
        m_battleManager.Encount(m_mapManager.NowMapID, cellType);
    }

    /// <summary>
    /// 階層の更新
    /// </summary>
    private void FloorFinished()
    {
        m_floor.Value++;
        Debug.Log($"現在フロア {m_floor.Value}");
    }
}

public enum GameState
{
    MapSelect,
    Battle,
}