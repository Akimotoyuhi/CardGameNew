using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.UI;

public class StockItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image m_image;
    [SerializeField] Sprite m_defaultSprite;
    [SerializeField] Text m_turnTextDisplay;
    private string m_tooltip;
    private List<Command> m_stockCommand = new List<Command>();
    private List<Command> m_releaseCommand = new List<Command>();
    private ReactiveProperty<int> m_turnTextValue = new ReactiveProperty<int>();
    public List<Command> Command => m_stockCommand;
    /// <summary>使用中かどうか</summary>
    public bool IsUsed { get; private set; }
    /// <summary>ストックしておくターン数</summary>
    public int StockTurn { get => m_turnTextValue.Value; private set => m_turnTextValue.Value = value; }
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

    public void Setup()
    {
        m_turnTextValue
            .Subscribe(i =>
            {
                if (i > 0)
                    m_turnTextDisplay.text = i.ToString();
                else
                    m_turnTextDisplay.text = "";
            }).AddTo(this);
        m_image.sprite = m_defaultSprite;
        IsUsed = false;
    }

    public void SetCommand(List<Command> command, Sprite sprite, string tooltip)
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
            m_tooltip = tooltip;
        });
        IsUsed = true;
    }

    public void Init()
    {
        m_stockCommand = new List<Command>();
        m_releaseCommand = new List<Command>();
        Setup();
    }

    //以下インターフェース

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.Instance.SetInfoText = m_tooltip;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.Instance.SetInfoText = "";
    }
}
