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
            //最初と最後はセル１つ
            if (i == 0 || i == m_nowMap.MaxColumn - 1)
                cellIndex = 1;
            //現在フロアのセル数を決める
            else
                cellIndex = Random.Range(m_nowMap.Chip.MinCellNum, m_nowMap.Chip.MaxCellNum);

            for (int n = 0; n < cellIndex; n++)
            {
                Cell cell = Instantiate(m_cellPrefab);
                col.AddCell = cell;
                cell.transform.SetParent(col.transform);
                //最後のマスはボスマスで固定
                if (i == m_nowMap.MaxColumn - 1)
                    cell.SetCellType = CellType.Boss;
                else
                    cell.SetCellType = m_nowMap.Chip.Lottery(i);
                cell.CellSubject.Subscribe(c => m_encount.OnNext(c)).AddTo(cell);
                cell.Floor = m_act * i + 1; //floorは１fからなのでズレを解決する
                cell.Setup();
            }
            m_columns.Add(col);
        }
    }
}
