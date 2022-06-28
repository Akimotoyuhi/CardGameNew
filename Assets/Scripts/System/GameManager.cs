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
    public System.IObservable<GameState> GameStateObservable => m_gameState;
    public string SetInfoText { set => m_infoText.Value = value; }
    public System.IObservable<string> InfoTextUpdate => m_infoText;
    public System.IObservable<int> FloorUpdate => m_floor;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        m_battleManager.Setup();
        m_battleManager.BattleFinished
            .Subscribe(_ => FloorFinished()).AddTo(m_battleManager);
        m_guiManager.Setup();
        m_mapManager.Setup();
        m_mapManager.EncountObservable.Subscribe(ct => Encount(ct)).AddTo(m_mapManager);
        m_gameState.Value = GameState.MapSelect;
        m_floor.Value = 1;
    }

    private void Encount(CellType cellType)
    {
        if (cellType == CellType.Rest)
        {
            Debug.Log("ãxåeÉ}ÉXñ¢é¿ëï");
            return;
        }
        m_gameState.Value = GameState.Battle;
        m_battleManager.Encount(m_mapManager.NowMapID, cellType);
    }

    private void FloorFinished()
    {
        m_floor.Value++;
        Debug.Log($"åªç›ÉtÉçÉA {m_floor.Value}");
    }
}

public enum GameState
{
    MapSelect,
    Battle,
}