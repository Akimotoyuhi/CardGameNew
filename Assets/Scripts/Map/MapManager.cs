using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class MapManager : MonoBehaviour
{
    [SerializeField] MapData m_mapData;
    [SerializeField] Column m_columnPrefab;
    [SerializeField] Cell m_cellPrefab;
    [SerializeField] Transform m_columnParent;
    [SerializeField] int m_maxCell;
    /// <summary>����act</summary>
    private int m_act = 1;
    /// <summary>���݃}�b�v</summary>
    private MapDataBase m_nowMap;
    private List<Column> m_columns = new List<Column>();

    public void Setup()
    {
        Create();
    }

    public void Create()
    {
        List<MapDataBase> databases = m_mapData.GetDataBases((Act)m_act-1);
        int r = Random.Range(0, databases.Count);//�Ƃ肠�����}�b�v�����_�����I�@��ɑI���ł���悤�ɂ���
        Debug.Log($"Count {databases.Count}, ���I���ꂽ���l{r}");
        m_nowMap = databases[r];
        for (int i = 0; i < m_nowMap.MaxColumn; i++)
        {
            Column col = Instantiate(m_columnPrefab);
            col.transform.SetParent(m_columnParent);
            col.SetFloor = i;
            for (int n = 0; n < m_maxCell; n++)
            {
                Cell cell = Instantiate(m_cellPrefab);
                col.AddCell = cell;
                cell.transform.SetParent(col.transform);
                //cell.SetCellType = m_nowMap.Chip[n].GetMapType;
                cell.CellSubject.Subscribe(c => CellClick(c)).AddTo(cell);
            }
            m_columns.Add(col);
        }
    }

    public void CellClick(CellType cellType)
    {
        Debug.Log($"{cellType}���N���b�N���ꂽ");
    }
}
