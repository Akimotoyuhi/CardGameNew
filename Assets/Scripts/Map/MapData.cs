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
    [SerializeField] List<MapChip> m_chip;
    public string Name => m_name;
    public MapID MapID => m_mapID;
    public Act Act => m_act;
    public int MaxColumn => m_chip.Count;
    public List<MapChip> Chip => m_chip;
}
[System.Serializable]
public class MapChip
{
    [SerializeField] List<DeteilSetting> m_type;
    [System.Serializable]
    public class DeteilSetting
    {
        [SerializeField] CellType m_cellType;
        [SerializeField, Range(0, 100)] int m_probability;
        public CellType Lottery
        {
            get
            {
                int r = Random.Range(0, 100);
                if (r >= m_probability)
                    return m_cellType;
                else
                    return CellType.None;
            }
        }
    }
    public CellType GetMapType
    {
        get
        {
            int r = Random.Range(0, m_type.Count);
            return m_type[r];
        }
    }
}
#region Enums
public enum MapID
{
    BlackCopperRuins,
    FreshRiver,
    SuperEden,
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