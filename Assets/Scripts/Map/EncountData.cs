using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EncountData")]
public class EncountData : ScriptableObject
{
    [SerializeField] List<EncountDataBase> m_database;
}
[System.Serializable]
public class EncountDataBase
{
    [SerializeField] MapID m_mapID;
    [SerializeField] FirstHarfEnemy m_firstHarfEnemy;
    [SerializeField] SecondHarfEnemy m_secondHarfEnemy;
    [SerializeField] Elite m_elite;


    [System.Serializable]
    public class FirstHarfEnemy
    {
        List<EnemyID> m_enemy;
        public List<EnemyID> GetEnemys => m_enemy;
    }
    [System.Serializable]
    public class SecondHarfEnemy
    {
        List<EnemyID> m_enemy;
        public List<EnemyID> GetEnemys => m_enemy;
    }
    [System.Serializable]
    public class Elite
    {
        List<EnemyID> m_elite;
        public List<EnemyID> GetElites => m_elite;
    }
    [System.Serializable]
    public class Boss
    {
        List<EnemyID> m_boss;
        public List<EnemyID> GetBosses => m_boss;
    }
}
