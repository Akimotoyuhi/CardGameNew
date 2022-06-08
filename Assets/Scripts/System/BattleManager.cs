using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] EnemyManager m_enemyManager;
    [SerializeField] Player m_playerPrefab;
    private Player m_currentPlayer;

    public void Setup()
    {
        m_enemyManager.Setup();
        m_currentPlayer = Instantiate(m_playerPrefab);
        m_currentPlayer.Setup();
    }
}
