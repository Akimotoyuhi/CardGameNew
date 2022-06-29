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
    /// <summary>�K�w�̍X�V��ʒm����</summary>
    private Subject<int> m_floorUpdateSubject = new Subject<int>();
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
            //�ŏ��ƍŌ�̓Z���P��
            if (i == 0 || i == m_nowMap.MaxColumn - 1)
                cellIndex = 1;
            //���݃t���A�̃Z���������߂�
            else
                cellIndex = Random.Range(m_nowMap.Chip.MinCellNum, m_nowMap.Chip.MaxCellNum);

            for (int n = 0; n < cellIndex; n++)
            {
                Cell cell = Instantiate(m_cellPrefab);
                col.AddCell = cell;
                cell.transform.SetParent(col.transform);
                //�Ō�̃}�X�̓{�X�}�X�ŌŒ�
                if (i == m_nowMap.MaxColumn - 1)
                    cell.SetCellType = CellType.Boss;
                else
                    cell.SetCellType = m_nowMap.Chip.Lottery(i);
                cell.CellSubject.Subscribe(c => m_encount.OnNext(c)).AddTo(cell);
                cell.Floor = m_act * i + 1; //floor�͂Pf����Ȃ̂ŃY������������
                cell.Setup();
            }
            m_columns.Add(col);
        }
    }
}
