using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;

public class CharactorManager : MonoBehaviour
{
    [Header("�v���C���[�֘A")]
    [SerializeField] PlayerID m_usePlayerID;
    [SerializeField] PlayerData m_playerData;
    [SerializeField] Player m_playerPrefab;
    [SerializeField] Transform m_playerParent;
    private Player m_currentPlayer;
    /// <summary>�v���C���[�̏��������J�[�h</summary>
    private CardClass m_cardClass;
    /// <summary>�g�p�J�[�h�^�C�v</summary>
    private CardClassType m_cardClassType;
    [Header("�G�֘A")]
    [SerializeField] EnemyData m_enemyData;
    [SerializeField] Enemy m_enemyPrefab;
    [SerializeField] Transform m_enemisParent;
    private List<Enemy> m_currentEnemies = new List<Enemy>();

    public Player CurrentPlayer => m_currentPlayer;
    public List<Enemy> CurrentEnemies => m_currentEnemies;
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
        m_currentPlayer.DeadSubject.Subscribe(_ => Debug.Log("�Q�[���I�[�o�[")).AddTo(this);

        Enemy e = Instantiate(m_enemyPrefab);
        e.transform.SetParent(m_enemisParent, false);
        e.SetBaseData(m_enemyData.Databases[0]); //�Ƃ肠����
        e.DeadSubject.Subscribe(_ => Debug.Log("�G�|����")).AddTo(this);
        m_currentEnemies.Add(e);
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
        foreach (var e in m_currentEnemies)
            await e.TurnEnd(turn);
        await CurrentPlayer.TurnEnd(turn);
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
}
