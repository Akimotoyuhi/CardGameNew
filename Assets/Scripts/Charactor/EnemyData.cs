using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData")]
public class EnemyData : ScriptableObject
{
    [SerializeField] List<EnemyDataBase> m_databases;
    public List<EnemyDataBase> Databases => m_databases;
}

[System.Serializable]
public class EnemyDataBase
{
    [SerializeField] string m_label;
    [SerializeField] EnemyID m_id;
    [SerializeField] CharactorDataBase m_characterData;
    [SerializeField] List<EnemyAI> m_enemyAi;
    public EnemyID EnemyID => m_id;
    public CharactorDataBase CharactorData => m_characterData;
    //public List<EnemyAI> EnemyAI => m_enemyAi;
    /// <summary>
    /// 行動条件を上から順に評価し、成功した行動内容を返す
    /// </summary>
    public List<Command> Action(Field field, Player player, Enemy enemy)
    {
        foreach (var ai in m_enemyAi)
        {
            Debug.Log(ai);
            if (ai.Conditional.Evaluation(field, player, enemy))
                return ai.EnemyCommands.Execute();
        }
        return null;
    }
}

[System.Serializable]
public class EnemyAI
{
    [SerializeField] Conditional m_conditional;
    [SerializeField] CommandSelect m_enemyCommands;
    public Conditional Conditional => m_conditional;
    public CommandSelect EnemyCommands => m_enemyCommands;
}

public enum EnemyID
{
    OriginiumSlag,
}