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
    public EnemyID EnemyID => m_id;
    public CharactorDataBase CharactorData => m_characterData;
}

public enum EnemyID
{
    OriginiumSlag,
}