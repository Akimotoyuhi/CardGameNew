using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// プレイヤー、敵の基底クラス
/// </summary>
public class CharactorManager : MonoBehaviour
{
    [Header("プレイヤー関連")]
    [SerializeField] PlayerID m_usePlayerID;
    [SerializeField] PlayerData m_playerData;
    [SerializeField] Player m_playerPrefab;
    [SerializeField] Transform m_playerParent;
    private Player m_currentPlayer;
    /// <summary>プレイヤーの初期所持カード</summary>
    private List<HaveCardData> m_haveCards;
    /// <summary>使用カードタイプ</summary>
    private CardClassType m_cardClassType;
    [Header("敵関連")]
    [SerializeField] EnemyData m_enemyData;
    [SerializeField] Enemy m_enemyPrefab;
    [SerializeField] Transform m_enemisParent;
    [SerializeField] float m_enemyFadeoutDuration;
    private List<Enemy> m_currentEnemies = new List<Enemy>();
    private Subject<Enemy> m_newEnemyCreateSubject = new Subject<Enemy>();
    private Subject<BattleEndType> m_battleEnd = new Subject<BattleEndType>();
    /// <summary>現在のプレイヤーのインスタンス</summary>
    public Player CurrentPlayer => m_currentPlayer;
    /// <summary>敵たち</summary>
    //public List<Enemy> CurrentEnemies => m_currentEnemies;
    /// <summary>所持カード</summary>
    public List<HaveCardData> HaveCard => m_haveCards;
    /// <summary>このゲームで使用されるカードクラス</summary>
    public CardClassType CardClassType => m_cardClassType;
    /// <summary>新たな敵が作られた際に通知する</summary>
    public System.IObservable<Enemy> NewEnemyCreateSubject => m_newEnemyCreateSubject;
    /// <summary>戦闘の終了を通知する</summary>
    public System.IObservable<BattleEndType> BattleEndSubject => m_battleEnd;

    public void Setup()
    {
        m_haveCards = m_playerData.DataBase[(int)m_usePlayerID].CardClassSelector.HaveCardData;
        Create();
    }

    /// <summary>
    /// 敵グループとプレイヤーを作る<br/>enemiesがnullの場合はプレイヤーだけ生成する
    /// </summary>
    public void Create(List<EnemyID> enemies = null)
    {
        //プレイヤーの生成
        if (m_currentPlayer == null)
        {
            m_currentPlayer = Instantiate(m_playerPrefab);
        }
        m_currentPlayer.transform.SetParent(m_playerParent, false);
        m_currentPlayer.SetBaseData(m_playerData.DataBase[(int)m_usePlayerID]);
        m_currentPlayer.DeadSubject.Subscribe(_ => BattleEnd(BattleEndType.Gameover)).AddTo(m_currentPlayer);
        m_cardClassType = m_playerData.DataBase[(int)m_usePlayerID].RewardClassType;

        if (enemies == null)
            return;
        //敵の生成
        enemies.ForEach(id =>
        {
            Enemy e = Instantiate(m_enemyPrefab);
            e.DeadSubject.Subscribe(_ => BattleEnd(BattleEndType.EnemiesDead)).AddTo(this);
            m_newEnemyCreateSubject.OnNext(e);
            e.transform.SetParent(m_enemisParent, false);
            e.SetBaseData(m_enemyData.Databases[(int)id]);
            e.DeadDuration = m_enemyFadeoutDuration;
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
        await CurrentPlayer.TurnEnd(turn);
        foreach (var e in m_currentEnemies)
            await e.TurnEnd(turn);
    }

    /// <summary>
    /// コマンドを各キャラクターに対し実行する
    /// </summary>
    /// <param name="cmds"></param>
    public void CommandExecutor(Command command)
    {
        switch (command.UseType)
        {
            case UseType.None:
                break;
            case UseType.Player:
                CurrentPlayer.Damage(command);
                break;
            case UseType.Enemy:
                m_currentEnemies[command.TargetEnemyIndex].Damage(command);
                break;
            case UseType.AllEnemies:
                m_currentEnemies.ForEach(e => e.Damage(command));
                break;
            case UseType.System:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// バトルの終了判定を行う
    /// </summary>
    private void BattleEnd(BattleEndType battleEndType)
    {
        switch (battleEndType)
        {
            case BattleEndType.EnemiesDead:
                //敵が全滅しているかを確認する　していたらバトル終了
                foreach (var e in m_currentEnemies)
                {
                    if (!e.IsDead)
                        return;
                }
                Init();
                break;
            default:
                break;
        }
        m_battleEnd.OnNext(battleEndType);
    }

    public void Init()
    {
        for (int i = m_currentEnemies.Count - 1; i >= 0; i--)
        {
            Destroy(m_currentEnemies[i].gameObject);
            m_currentEnemies.RemoveAt(i);
        }
    }
}
public enum BattleEndType
{
    EnemiesDead,
    Gameover,
}