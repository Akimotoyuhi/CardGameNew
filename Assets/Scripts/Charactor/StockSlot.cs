using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

/// <summary>
/// �L�����N�^�[�̃X�g�b�N�X���b�g�̊Ǘ�
/// </summary>
public class StockSlot : MonoBehaviour
{
    [SerializeField] StockItem m_stockItemPrefab;
    [SerializeField] Transform m_stockItemParent;
    [SerializeField] int m_maxSlot;
    private List<StockItem> m_stockItems = new List<StockItem>();

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

    /// <summary>�X�g�b�N�������ʂ�ǉ�����</summary>
    public void Add(List<Command> command, Sprite sprite, string tooltip)
    {
        m_stockItems.ForEach(item =>
        {
            if (!item.IsUsed)
            {
                item.Setup(command, sprite, tooltip);
                return;
            }
        });
    }
}
