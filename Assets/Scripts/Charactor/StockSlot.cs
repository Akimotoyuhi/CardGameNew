using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

/// <summary>
/// キャラクターのストックスロットの管理
/// </summary>
public class StockSlot : MonoBehaviour
{
    [SerializeField] StockItem m_stockItemPrefab;
    [SerializeField] Transform m_stockItemParent;
    [SerializeField] int m_maxSlot;
    private List<StockItem> m_stockItems = new List<StockItem>();
    public List<StockItem> StockItems => m_stockItems;

    public void Setup()
    {
        if (m_stockItems.Count <= 0)
        {
            for (int i = 0; i < m_maxSlot; i++)
            {
                StockItem item = Instantiate(m_stockItemPrefab);
                item.Setup();
                m_stockItems.Add(item);
                item.transform.SetParent(m_stockItemParent, false);
            }
        }
    }

    /// <summary>ストックされる効果を追加する<br/>追加の成否を返す</summary>
    public bool Add(List<Command> command, Sprite sprite, string tooltip)
    {
        foreach (var item in m_stockItems)
        {
            if (!item.IsUsed)
            {
                item.Setup(command, sprite, tooltip);
                return true;
            }
        }
        return false;
    }
}
