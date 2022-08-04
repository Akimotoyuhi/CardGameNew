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
    [SerializeField] List<StockItem> m_stockItems;

    public void Setup()
    {

    }

    /// <summary>�X�g�b�N�������ʂ�ǉ�����</summary>
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
