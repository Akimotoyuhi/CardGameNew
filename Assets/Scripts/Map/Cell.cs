using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    [SerializeField] Image m_image;
    [SerializeField] Button m_button;
    [SerializeField] Sprite m_battleSprite;
    [SerializeField] Sprite m_restSprite;
    [SerializeField] Sprite m_eliteSprite;
    [SerializeField] Sprite m_bossSprite;
    private CellType m_cellType;
    private Subject<CellType> m_cellSubject = new Subject<CellType>();
    public CellType SetCellType
    {
        set
        {
            m_cellType = value;
            SetSprite();
        }
    }
    public IObservable<CellType> CellSubject => m_cellSubject;

    public void Setup()
    {
        m_button.onClick.AddListener(() => m_cellSubject.OnNext(m_cellType));
    }

    private void SetSprite()
    {
        switch (m_cellType)
        {
            case CellType.None:
                m_image.color = Color.clear;
                m_button.interactable = false;
                break;
            case CellType.FirstHalfBattle:
                m_image.sprite = m_battleSprite;
                break;
            case CellType.SecondHalfBattle:
                m_image.sprite = m_battleSprite;
                break;
            case CellType.Elite:
                m_image.sprite = m_eliteSprite;
                break;
            case CellType.Boss:
                m_image.sprite = m_bossSprite;
                break;
            case CellType.Rest:
                m_image.sprite = m_restSprite;
                break;
            default:
                break;
        }
    }
}
