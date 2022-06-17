using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class GameManager : MonoBehaviour
{
    [SerializeField] BattleManager m_battleManager;
    [SerializeField] GUIManager m_guiManager;
    [SerializeField] MapManager m_mapManager;
    public static GameManager Instance { get; private set; }

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
    }

    private void Encount(CellType cellType)
    {
        if (cellType == CellType.Rest)
        {
            Debug.Log("‹xŒeƒ}ƒX–¢ŽÀ‘•");
            return;
        }
        m_battleManager.Encount(m_mapManager.NowMapID, cellType);
    }
}
