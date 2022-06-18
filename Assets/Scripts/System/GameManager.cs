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
    public static GameManager Instance { get; private set; }
    public System.IObservable<GameState> GameStateObservable => m_gameState;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        m_battleManager.Setup();
        m_guiManager.Setup();
        m_mapManager.Setup();
        m_mapManager.EncountObservable.Subscribe(ct => Encount(ct)).AddTo(m_mapManager);
        m_gameState.Value = GameState.MapSelect;
    }

    private void Encount(CellType cellType)
    {
        if (cellType == CellType.Rest)
        {
            Debug.Log("‹xŒeƒ}ƒX–¢ŽÀ‘•");
            return;
        }
        m_gameState.Value = GameState.Battle;
        m_battleManager.Encount(m_mapManager.NowMapID, cellType);
    }
}

public enum GameState
{
    MapSelect,
    Battle,
}