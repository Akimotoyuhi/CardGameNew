using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// �v���C���[�A�G�̊��N���X
/// </summary>
public class CharactorManager : MonoBehaviour
{
    [Header("�v���C���[�֘A")]
    [SerializeField] PlayerID m_usePlayerID;
    [SerializeField] PlayerData m_playerData;
    [SerializeField] Player m_playerPrefab;
    [SerializeField] Transform m_playerParent;
    private Player m_currentPlayer;
    /// <summary>�v���C���[�̏��������J�[�h</summary>
    private List<HaveCardData> m_haveCards;
    /// <summary>�g�p�J�[�h�^�C�v</summary>
    private CardClassType m_cardClassType;
    [Header("�G�֘A")]
    [SerializeField] EnemyData m_enemyData;
    [SerializeField] Enemy m_enemyPrefab;
    [SerializeField] Transform m_enemisParent;
    [SerializeField] float m_enemyFadeoutDuration;
    private List<Enemy> m_currentEnemies = new List<Enemy>();
    private Subject<Enemy> m_newEnemyCreateSubject = new Subject<Enemy>();
    private Subject<BattleEndType> m_battleEnd = new Subject<BattleEndType>();
    /// <summary>���݂̃v���C���[�̃C���X�^���X</summary>
    public Player CurrentPlayer => m_currentPlayer;
    /// <summary>�G����</summary>
    //public List<Enemy> CurrentEnemies => m_currentEnemies;
    /// <summary>�����J�[�h</summary>
    public List<HaveCardData> HaveCard => m_haveCards;
    public System.IObservable<Enemy> NewEnemyCreateSubject => m_newEnemyCreateSubject;
    public System.IObservable<BattleEndType> BattleEndSubject => m_battleEnd;

    public void Setup()
    {
        m_haveCards = m_playerData.DataBase[(int)m_usePlayerID].CardClassSelector.HaveCardData;
        Create();
    }

    /// <summary>
    /// �G�O���[�v�ƃv���C���[�����<br/>enemies��null�̏ꍇ�̓v���C���[������������
    /// </summary>
    public void Create(List<EnemyID> enemies = null)
    {
        //�v���C���[�̐���
        if (m_currentPlayer == null)
        {
            m_currentPlayer = Instantiate(m_playerPrefab);
        }
        m_currentPlayer.transform.SetParent(m_playerParent, false);
        m_currentPlayer.SetBaseData(m_playerData.DataBase[(int)m_usePlayerID]);
        m_currentPlayer.DeadSubject.Subscribe(_ => Debug.Log("�Q�[���I�[�o�[")).AddTo(m_currentPlayer);

        if (enemies == null)
            return;
        //�G�̐���
        enemies.ForEach(id =>
        {
            Enemy e = Instantiate(m_enemyPrefab);
            e.transform.SetParent(m_enemisParent, false);
            e.SetBaseData(m_enemyData.Databases[(int)id]);
            e.DeadDuration = m_enemyFadeoutDuration;
            e.DeadSubject.Subscribe(_ => BattleEnd()).AddTo(this);
            m_newEnemyCreateSubject.OnNext(e);
            m_currentEnemies.Add(e);
        });
    }

    public async UniTask TurnBegin(int turn)
    {
        Debug.Log("�^�[���J�n");
        foreach (var e in m_currentEnemies)
            await e.TurnBegin(turn);
        await CurrentPlayer.TurnBegin(turn);
    }

    public async UniTask TurnEnd(int turn)
    {
        Debug.Log("�^�[���I��");
        await CurrentPlayer.TurnEnd(turn);
        foreach (var e in m_currentEnemies)
            await e.TurnEnd(turn);
    }

    /// <summary>
    /// �R�}���h���e�L�����N�^�[�ɑ΂����s����
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

    /// <summary>
    /// �o�g���̏I��������s�Ȃ�
    /// </summary>
    private void BattleEnd()
    {
        //�G���S�ł��Ă��邩���m�F����@���Ă�����o�g���I��
        foreach (var e in m_currentEnemies)
        {
            if (!e.IsDead)
                return;
        }
        m_battleEnd.OnNext(BattleEndType.EnemiesDead);
    }
}
public enum BattleEndType
{
    EnemiesDead,
    Gameover,
}