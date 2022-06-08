using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorManager : MonoBehaviour
{
    [Header("プレイヤー関連")]
    [SerializeField] PlayerData m_playerData;
    [SerializeField] Player m_playerPrefab;
    [SerializeField] Transform m_playerParent;
    private Player m_currentPlayer;
    [Header("敵関連")]
    [SerializeField] EnemyData m_enemyData;
    [SerializeField] Enemy m_enemyPrefab;
    [SerializeField] Transform m_enemisParent;
    private List<Enemy> m_currentEnemies = new List<Enemy>();

    public void Setup()
    {
        Create();
    }

    public void Create()
    {
        m_currentPlayer = Instantiate(m_playerPrefab);
        m_currentPlayer.transform.SetParent(m_playerParent, false);
        m_currentPlayer.SetBaseData(m_playerData.DataBase[0]); //とりあえず

        Enemy e = Instantiate(m_enemyPrefab);
        e.transform.SetParent(m_enemisParent, false);
        e.SetBaseData(m_enemyData.Databases[0]); //とりあえず
        m_currentEnemies.Add(e);
    }
}
