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
    private List<Command> m_stockCommand = new List<Command>();
    private List<Command> m_releaseCommand = new List<Command>();
    public List<Command> Command => m_stockCommand;
    /// <summary>使用中かどうか</summary>
    public bool IsUsed { get; private set; }
    /// <summary>ストックしておくターン数</summary>
    public int StockTurn { get; private set; }
    public List<Command> ExecuteStockCommand
    {
        get
        {
            if (StockTurn <= 0)
                return new List<Command>();
            StockTurn--;
            return m_stockCommand;
        }
    }
    public List<Command> ExecuteStockReleaseCommand
    {
        get
        {
            if (StockTurn <= 0)
                return m_releaseCommand;
            return new List<Command>();
        }
    }
    public void Setup(List<Command> command, Sprite sprite, string tooltip)
    {
        m_image.sprite = sprite;
        command.ForEach(c =>
        {
            if (c.IsStockRelease)
            {
                c.IsStockRelease = false;
                m_releaseCommand.Add(c);
            }
            else
            {
                StockTurn = c.StockTurn;
                c.StockTurn = -1;
                m_stockCommand.Add(c);
            }
        });
        IsUsed = true;
    }

    public void Setup()
    {
        m_image.sprite = m_defaultSprite;
        IsUsed = false;
    }

    public void Init()
    {
        m_stockCommand = new List<Command>();
        m_releaseCommand = new List<Command>();
        Setup();
    }
}
