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
    /// <summary>現在act</summary>
    private int m_act = 1;
    /// <summary>現在マップ</summary>
    private MapDataBase m_nowMap;
    private List<Column> m_columns = new List<Column>();
    /// <summary>階層の更新を通知する</summary>
    private Subject<int> m_floorUpdateSubject = new Subject<int>();
    public MapID NowMapID => m_nowMap.MapID;
    public System.IObservable<CellType> EncountObservable => m_encount;

    public void Setup()
    {
        Create();
    }

    /// <summary>
    /// マップ生成
    /// </summary>
    public void Create()
    {
        List<MapDataBase> databases = m_mapData.GetDataBases((Act)m_act - 1);
        int r = Random.Range(0, databases.Count);//とりあえずマップランダム抽選　後に選択できるようにする
        m_nowMap = databases[r];
        for (int i = 0; i < m_nowMap.MaxColumn; i++)
        {
            Column col = Instantiate(m_columnPrefab);
            col.transform.SetParent(m_columnParent);
            col.SetFloor = i;
            int cellIndex;
            if (i == 0 || i == m_nowMap.MaxColumn - 1)//最初と最後はセル１つ
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

    private void CellClick(CellType cellType)
    {
        m_encount.OnNext(cellType);
    }
}
