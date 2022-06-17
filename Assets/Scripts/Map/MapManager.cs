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
    private Subject<CellType> m_encount = new Subject<CellType>();
    /// <summary>����act</summary>
    private int m_act = 1;
    /// <summary>���݃}�b�v</summary>
    private MapDataBase m_nowMap;
    private List<Column> m_columns = new List<Column>();
    public MapID NowMapID => m_nowMap.MapID;
    public System.IObservable<CellType> EncountObservable => m_encount;

    public void Setup()
    {
        Create();
    }

    /// <summary>
    /// �}�b�v����
    /// </summary>
    public void Create()
    {
        List<MapDataBase> databases = m_mapData.GetDataBases((Act)m_act - 1);
        int r = Random.Range(0, databases.Count);//�Ƃ肠�����}�b�v�����_�����I�@��ɑI���ł���悤�ɂ���
        m_nowMap = databases[r];
        for (int i = 0; i < m_nowMap.MaxColumn; i++)
        {
            Column col = Instantiate(m_columnPrefab);
            col.transform.SetParent(m_columnParent);
            col.SetFloor = i;
            int cellIndex;
            if (i == 0 || i == m_nowMap.MaxColumn - 1)//�ŏ��ƍŌ�̓Z���P��
                cellIndex = 1;
            else
                cellIndex = Random.Range(m_nowMap.Chip.MinCellNum, m_nowMap.Chip.MaxCellNum);
            for (int n = 0; n < cellIndex; n++)
            {
                Cell cell = Instantiate(m_cellPrefab);
                col.AddCell = cell;
                cell.transform.SetParent(col.transform);
                if (i == m_nowMap.MaxColumn - 1)
                    cell.SetCellType = CellType.Boss;
                else
                    cell.SetCellType = m_nowMap.Chip.Lottery(i);
                cell.CellSubject.Subscribe(c => CellClick(c)).AddTo(cell);
                cell.Setup();
            }
            m_columns.Add(col);
        }
    }

    public void CellClick(CellType cellType)
    {
        m_encount.OnNext(cellType);
    }
}
