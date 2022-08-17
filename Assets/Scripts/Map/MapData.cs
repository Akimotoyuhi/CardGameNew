using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData")]
public class MapData : ScriptableObject
{
    [SerializeField] List<MapID> m_firstMapPath;
    [SerializeField] List<MapDataBase> m_mapDataBase;
    public List<MapDataBase> GetDataBases(Act act)
    {
        List<MapDataBase> ret = new List<MapDataBase>();
        m_mapDataBase.ForEach(m =>
        {
            if (m.Act == act)
                ret.Add(m);
        });
        return ret;
    }
    public MapDataBase GetDataBases(MapID mapID)
    {
        foreach (var m in m_mapDataBase)
        {
            if (m.MapID == mapID)
                return m;
        }
        throw new System.IndexOutOfRangeException("�}�b�v�f�[�^��������܂���ł���");
    }
}

[System.Serializable]
public class MapDataBase
{
    [SerializeField] string m_name;
    [SerializeField] MapID m_mapID;
    [SerializeField] Act m_act;
    [SerializeField] int m_maxColumn;
    [SerializeField] bool m_isTerminal;
    [SerializeField] List<MapID> m_nextMap;
    [SerializeField] MapChip m_chip;
    /// <summary>�}�b�v��</summary>
    public string Name => m_name;
    /// <summary>�}�b�vID</summary>
    public MapID MapID => m_mapID;
    /// <summary>����Act</summary>
    public Act Act => m_act;
    /// <summary>�ő�Z����</summary>
    public int MaxColumn => m_maxColumn;
    /// <summary>�I�[�t���O</summary>
    public bool IsTerminal => m_isTerminal;
    /// <summary>���}�b�v</summary>
    public List<MapID> NextMapID => m_nextMap;
    /// <summary>�}�b�v�̏ڍ�</summary>
    public MapChip Chip => m_chip;
}
[System.Serializable]
public class MapChip
{
    [SerializeField, Tooltip("�Z�������̍ŏ���")] int m_minCellNum = 1;
    [SerializeField, Tooltip("�Z�������̍ő吔")] int m_maxCellNum = 3;
    [SerializeField, Tooltip("�ǂ�index����㔼�Ƃ��邩")] int m_secondHalfIndex;
    [SerializeField] DetailSetting m_detailSetting;
    [System.Serializable]
    public class DetailSetting
    {
        [SerializeField, Tooltip("�x�e�}�X�𐶐�����ŏ��ʒu"), Header("�x�e�}�X�̏ڍאݒ�")] int m_restMinIndex;
        [SerializeField, Tooltip("�x�e�}�X�𐶐�����ő�ʒu")] int m_restMaxIndex;
        [SerializeField, Tooltip("�x�e�}�X�̐����m��"), Range(0, 100)] int m_restProbability;
        [SerializeField, Tooltip("�x�e�}�X���΂ɐ�������ʒu\n�������-1�Ɠ���")] int m_restAbsoluteIndex;
        [SerializeField, Tooltip("�G���[�g�}�X�𐶐�����ŏ��ʒu"), Header("�G���[�g�}�X�̏ڍאݒ�")] int m_eliteMinIndex;
        [SerializeField, Tooltip("�G���[�g�}�X�𐶐�����ő�ʒu")] int m_eliteMaxIndex;
        [SerializeField, Tooltip("�G���[�g�}�X�̐����m��"), Range(0, 100)] int m_eliteProbability;
        [SerializeField, Tooltip("�G���[�g�}�X���΂ɐ�������ʒu\n�������-1�Ɠ���")] int m_eliteAbsoluteIndex;
        public bool RestCellLottery(int index)
        {
            if (m_restAbsoluteIndex == index)
                return true;
            if (index >= m_restMinIndex && index <= m_restMaxIndex)
            {
                int r = Random.Range(0, 100);
                if (r <= m_restProbability)
                    return true;
            }
            return false;
        }
        public bool EliteCellLottery(int index)
        {
            if (m_eliteAbsoluteIndex == index)
                return true;
            if (index >= m_eliteMinIndex && index <= m_eliteMaxIndex)
            {
                int r = Random.Range(0, 100);
                if (r <= m_eliteProbability)
                    return true;
            }
            return false;
        }
    }
    public int MinCellNum => m_minCellNum;
    public int MaxCellNum => m_maxCellNum + 1;
    /// <summary>
    /// �}�X���I
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public CellType Lottery(int index)
    {
        if (m_detailSetting.RestCellLottery(index))
            return CellType.Rest;
        if (m_detailSetting.EliteCellLottery(index))
            return CellType.Elite;
        if (index >= m_secondHalfIndex)
            return CellType.SecondHalfBattle;
        return CellType.FirstHalfBattle;
    }
}
#region Enums
public enum MapID
{
    /// <summary>���</summary>
    Ruin,
    /// <summary>����₩��</summary>
    FreshRiver,
    /// <summary>�s��R��</summary>
    SteepMountainRoad,
    /// <summary>�A�E�g�G���AX11</summary>
    OutEriaX11,
}
public enum Act
{
    First,
    Second,
    Third,
}
public enum CellType
{
    None = -1,
    FirstHalfBattle,
    SecondHalfBattle,
    Elite,
    Boss,
    Rest,
}
#endregion