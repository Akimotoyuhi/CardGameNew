using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EncountData")]
public class EncountData : ScriptableObject
{
    [SerializeField] List<EncountDataBase> m_database;
    /// <summary>
    /// �w�肵���}�b�v�̑S�Ă̓G�O���[�v���擾
    /// </summary>
    /// <param name="mapID"></param>
    /// <returns></returns>
    /// <exception cref="System.Exception"></exception>
    public EncountDataBase GetEncountData(MapID mapID)
    {
        foreach (var db in m_database)
        {
            if (db.MapID == mapID)
                return db;
        }
        throw new System.Exception("�w�肳�ꂽMapID�ɑΉ������G���J�E���g�f�[�^����������܂���");
    }
}
[System.Serializable]
public class EncountDataBase
{
    [SerializeField] MapID m_mapID;
    [SerializeField] List<FirstHarfEnemy> m_firstHarfEnemy;
    [SerializeField] List<SecondHarfEnemy> m_secondHarfEnemy;
    [SerializeField] List<Elite> m_elite;
    [SerializeField] List<Boss> m_boss;
    public MapID MapID => m_mapID;
    /// <summary>
    /// �G�O���[�v�̎擾
    /// </summary>
    /// <param name="cellType"></param>
    /// <returns></returns>
    public List<EnemyID> GetEnemies(CellType cellType)
    {
        List<EnemyID> ret = null;
        int r;
        switch (cellType)
        {
            case CellType.FirstHalfBattle:
                r = Random.Range(0, m_firstHarfEnemy.Count);
                ret = m_firstHarfEnemy[r].GetEnemys;
                break;
            case CellType.SecondHalfBattle:
                r = Random.Range(0, m_secondHarfEnemy.Count);
                ret = m_secondHarfEnemy[r].GetEnemys;
                break;
            case CellType.Elite:
                r = Random.Range(0, m_elite.Count);
                ret = m_elite[r].GetEnemys;
                break;
            case CellType.Boss:
                r = Random.Range(0, m_boss.Count);
                ret = m_boss[r].GetEnemys;
                break;
            default:
                Debug.LogError("�G���J�E���g�f�[�^�����݂��Ȃ�Cell�ł�");
                break;
        }
        return ret;
    }

    [System.Serializable]
    public class FirstHarfEnemy
    {
        [SerializeField] List<EnemyID> m_enemy;
        public List<EnemyID> GetEnemys => m_enemy;
    }
    [System.Serializable]
    public class SecondHarfEnemy
    {
        [SerializeField] List<EnemyID> m_enemy;
        public List<EnemyID> GetEnemys => m_enemy;
    }
    [System.Serializable]
    public class Elite
    {
        [SerializeField] List<EnemyID> m_elite;
        public List<EnemyID> GetEnemys => m_elite;
    }
    [System.Serializable]
    public class Boss
    {
        [SerializeField] List<EnemyID> m_boss;
        public List<EnemyID> GetEnemys => m_boss;
    }
}