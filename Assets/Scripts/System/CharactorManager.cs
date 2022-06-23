using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;

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
    private Subject<Enemy> m_newEnemyCreateSubject = new Subject<Enemy>();
    /// <summary>現在のプレイヤーのインスタンス</summary>
    public Player CurrentPlayer => m_currentPlayer;
    /// <summary>敵たち</summary>
    //public List<Enemy> CurrentEnemies => m_currentEnemies;
    /// <summary>このゲーム中使用しているのカードクラス</summary>
    public CardClass CardClass => m_cardClass;
    /// <summary>このゲーム中使用しているのカードクラス(Enum)</summary>
    public CardClassType CardClassType => m_cardClassType;
    public System.IObservable<Enemy> NewEnemyCreateSubject => m_newEnemyCreateSubject;

    public void Setup()
    {
        Create();
    }

    public void Create(List<EnemyID> enemies = null)
    {
        if (m_currentPlayer == null)
        {
            m_currentPlayer = Instantiate(m_playerPrefab);
        }
        m_currentPlayer.transform.SetParent(m_playerParent, false);
        m_currentPlayer.SetBaseData(m_playerData.DataBase[(int)m_usePlayerID]);
        m_cardClass = m_playerData.DataBase[(int)m_usePlayerID].CardClassSelector.CardClass;
        m_cardClassType = m_playerData.DataBase[(int)m_usePlayerID].CardClassSelector.CardClass.CardClassType;
        m_currentPlayer.DeadSubject.Subscribe(_ => Debug.Log("ゲームオーバー")).AddTo(m_currentPlayer);

        if (enemies == null)
            return;
        enemies.ForEach(id =>
        {
            Enemy e = Instantiate(m_enemyPrefab);
            e.transform.SetParent(m_enemisParent, false);
            e.SetBaseData(m_enemyData.Databases[(int)id]); //とりあえず
            e.DeadSubject.Subscribe(_ => Debug.Log("敵倒した")).AddTo(this);
            m_newEnemyCreateSubject.OnNext(e);
            m_currentEnemies.Add(e);
        });
    }

    public async UniTask TurnBegin(int turn)
    {
        Debug.Log("ターン開始");
        foreach (var e in m_currentEnemies)
            await e.TurnBegin(turn);
        await CurrentPlayer.TurnBegin(turn);
    }

    public async UniTask TurnEnd(int turn)
    {
        Debug.Log("ターン終了");
        foreach (var e in m_currentEnemies)
            await e.TurnEnd(turn);
        await CurrentPlayer.TurnEnd(turn);
    }

    /// <summary>
    /// コマンドを各キャラクターに対し実行する
    /// </summary>
    /// <param name="cmds"></param>
    public void CommandExecutor(List<Command> cmds)
    {
        cmds.ForEach(c =>
        {
            switch (c.UseType)
            {
                case UseType.None:
                    break;
                case UseType.Player:
                    CurrentPlayer.Damage(c);
                    break;
                case UseType.Enemy:
                    m_currentEnemies[c.TargetEnemyIndex].Damage(c);
                    break;
                case UseType.AllEnemies:
                    m_currentEnemies.ForEach(e => e.Damage(c));
                    break;
                case UseType.System:
                    break;
                default:
                    break;
            }
        });
    }
}
