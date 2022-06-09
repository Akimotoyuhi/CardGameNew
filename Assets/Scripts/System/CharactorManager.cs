using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class CharactorManager : MonoBehaviour
{
    [Header("プレイヤー関連")]
    [SerializeField] PlayerID m_usePlayerID;
    [SerializeField] PlayerData m_playerData;
    [SerializeField] Player m_playerPrefab;
    [SerializeField] Transform m_playerParent;
    private Player m_currentPlayer;
    /// <summary>プレイヤーの初期所持カード</summary>
    private CardClass m_cardClass;
    /// <summary>使用カードタイプ</summary>
    private CardClassType m_cardClassType;
    [Header("敵関連")]
    [SerializeField] EnemyData m_enemyData;
    [SerializeField] Enemy m_enemyPrefab;
    [SerializeField] Transform m_enemisParent;
    private List<Enemy> m_currentEnemies = new List<Enemy>();
    public Player CurrentPlayer => m_currentPlayer;
    public List<Enemy> Enemies => m_currentEnemies;
    public CardClass CardClass => m_cardClass;
    public CardClassType CardClassType => m_cardClassType;

    public void Setup()
    {
        Create();
    }

    public void Create()
    {
        m_currentPlayer = Instantiate(m_playerPrefab);
        m_currentPlayer.transform.SetParent(m_playerParent, false);
        m_currentPlayer.SetBaseData(m_playerData.DataBase[(int)m_usePlayerID]);
        m_cardClass = m_playerData.DataBase[(int)m_usePlayerID].CardClassSelector.CardClass;
        m_cardClassType = m_playerData.DataBase[(int)m_usePlayerID].CardClassSelector.CardClass.CardClassType;
        m_currentPlayer.DeadSubject.Subscribe(_ => Debug.Log("ゲームオーバー")).AddTo(this);

        Enemy e = Instantiate(m_enemyPrefab);
        e.transform.SetParent(m_enemisParent, false);
        e.SetBaseData(m_enemyData.Databases[0]); //とりあえず
        e.DeadSubject.Subscribe(_ => Debug.Log("敵倒した")).AddTo(this);
        m_currentEnemies.Add(e);
    }

    /// <summary>
    /// コマンドを各キャラクターに対し実行する
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
