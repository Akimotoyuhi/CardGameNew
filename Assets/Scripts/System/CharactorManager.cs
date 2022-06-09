using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorManager : MonoBehaviour
{
    [Header("�v���C���[�֘A")]
    [SerializeField] PlayerData m_playerData;
    [SerializeField] Player m_playerPrefab;
    [SerializeField] Transform m_playerParent;
    private Player m_currentPlayer;
    [Header("�G�֘A")]
    [SerializeField] EnemyData m_enemyData;
    [SerializeField] Enemy m_enemyPrefab;
    [SerializeField] Transform m_enemisParent;
    private List<Enemy> m_currentEnemies = new List<Enemy>();
    public Player CurrentPlayer => m_currentPlayer;
    public List<Enemy> Enemies => m_currentEnemies;

    public void Setup()
    {
        Create();
    }

    public void Create()
    {
        m_currentPlayer = Instantiate(m_playerPrefab);
        m_currentPlayer.transform.SetParent(m_playerParent, false);
        m_currentPlayer.SetBaseData(m_playerData.DataBase[0]); //�Ƃ肠����

        Enemy e = Instantiate(m_enemyPrefab);
        e.transform.SetParent(m_enemisParent, false);
        e.SetBaseData(m_enemyData.Databases[0]); //�Ƃ肠����
        m_currentEnemies.Add(e);
    }

    /// <summary>
    /// �R�}���h���e�L�����N�^�[�ɑ΂����s����
    /// </summary>
    /// <param name="cmds"></param>
    public void CommandExecutor(List<Command> cmds)
    {
        cmds.ForEach(c =>
        {
            CurrentPlayer.Damage(c);
        });
    }
}
