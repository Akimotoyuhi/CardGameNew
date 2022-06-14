using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Column : MonoBehaviour
{
    private int m_columnFloor;
    private List<Cell> m_cells = new List<Cell>();
    public int SetFloor { set => m_columnFloor = value; }
    public Cell AddCell { set => m_cells.Add(value); }

    public void SetCellState(int floor)
    {

    }
}
