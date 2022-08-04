using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

/// <summary>
/// キャラクターのストックスロットの管理
/// </summary>
public class StockSlot : MonoBehaviour
{
    [SerializeField] List<StockItem> m_stockItems;

    public void Setup()
    {

    }

    /// <summary>ストックされる効果を追加する</summary>
    public void Add(Command command, Sprite sprite)
    {
        m_stockItems.ForEach(item =>
        {
            if (!item.IsUsed)
            {
                item.Setup(command, sprite);
                return;
            }
        });
    }
}
