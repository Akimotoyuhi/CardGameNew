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
    private Command m_command;
    public Command Command => m_command;
    /// <summary>使用中かどうか</summary>
    public bool IsUsed { get; private set; }

    public void Setup(Command command)
    {
        m_image.sprite = command.StockSprite;
        m_command = command;
        IsUsed = true;
    }

    public void Setup()
    {
        m_image.sprite = m_defaultSprite;
        IsUsed = false;
    }
}
