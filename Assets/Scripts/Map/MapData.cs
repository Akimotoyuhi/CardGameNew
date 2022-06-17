using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData")]
public class MapData : ScriptableObject
{
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
        throw new System.IndexOutOfRangeException("マップデータが見つかりませんでした");
    }
}

[System.Serializable]
public class MapDataBase
{
    [SerializeField] string m_name;
    [SerializeField] MapID m_mapID;
    [SerializeField] Act m_act;
    [SerializeField] int m_maxColumn;
    [SerializeField] MapChip m_chip;
    public string Name => m_name;
    public MapID MapID => m_mapID;
    public Act Act => m_act;
    public int MaxColumn => m_maxColumn;
    public MapChip Chip => m_chip;
}
[System.Serializable]
public class MapChip
{
    [SerializeField, Tooltip("セル生成の最小数")] int m_minCellNum = 1;
    [SerializeField, Tooltip("セル生成の最大数")] int m_maxCellNum = 3;
    [SerializeField, Tooltip("どのindexから後半とするか")] int m_secondHalfIndex;
    [SerializeField] DetailSetting m_detailSetting;
    [System.Serializable]
    public class DetailSetting
    {
        [SerializeField, Tooltip("休憩マスを生成する最小位置"), Header("休憩マスの詳細設定")] int m_restMinIndex;
        [SerializeField, Tooltip("休憩マスを生成する最大位置")] int m_restMaxIndex;
        [SerializeField, Tooltip("休憩マスの生成確立"), Range(0, 100)] int m_restProbability;
        [SerializeField, Tooltip("休憩マスを絶対に生成する位置\n無ければ-1と入力")] int m_restAbsoluteIndex;
        [SerializeField, Tooltip("エリートマスを生成する最小位置"), Header("エリートマスの詳細設定")] int m_eliteMinIndex;
        [SerializeField, Tooltip("エリートマスを生成する最大位置")] int m_eliteMaxIndex;
        [SerializeField, Tooltip("エリートマスの生成確立"), Range(0, 100)] int m_eliteProbability;
        [SerializeField, Tooltip("エリートマスを絶対に生成する位置\n無ければ-1と入力")] int m_eliteAbsoluteIndex;
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
    /// マス抽選
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
    /// <summary>黒銅遺跡</summary>
    BlackCopperRuins,
    /// <summary>さわやか川</summary>
    FreshRiver,
    /// <summary>スーパーエデン</summary>
    SuperEden,
    /// <summary>アウトエリアX11</summary>
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