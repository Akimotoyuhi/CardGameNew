using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData")]
public class MapData : ScriptableObject
{
    [SerializeField] List<MapDataBase> m_mapDataBase;
    public List<MapDataBase> MapDataBase => m_mapDataBase;
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
    public List<MapChip> Chip => m_chip;
}
[System.Serializable]
public class MapChip
{
    [SerializeField] List<MapType> m_type;
    public MapType GetMapType
    {
        get
        {
            int r = Random.Range(0, m_type.Count);
            return m_type[r];
        }
    }
}
public enum MapID
{
    YamadaElectric,
    FreshRiver,
    SuperEden,
}
public enum Act
{
    First,
    Second,
    Third,
}
public enum MapType
{
    None = -1,
    FirstHalfBattle,
    SecondHalfBattle,
    Elite,
    Boss,
    Rest,
}
