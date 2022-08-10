using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.UI;

public class StockItem : MonoBehaviour
{
    [SerializeField] Image m_image;
    [SerializeField] Sprite m_defaultSprite;
    private List<Command> m_command;
    public List<Command> Command => m_command;
    /// <summary>�g�p�����ǂ���</summary>
    public bool IsUsed { get; private set; }

    public void Setup(List<Command> command, Sprite sprite, string tooltip)
    {
        m_image.sprite = sprite;
        m_command = command;
        IsUsed = true;
    }

    public void Setup()
    {
        m_image.sprite = m_defaultSprite;
        IsUsed = false;
    }

    public List<Command> ExecuteStockCommand()
    {
        return null; //�Ƃ肠����
    }

    public List<Command> ExecuteStockReleaseCommand()
    {
        return null; //�Ƃ肠����
    }
}
