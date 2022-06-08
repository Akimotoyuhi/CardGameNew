using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;

public class Card : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    [SerializeField] Text m_nameText;
    [SerializeField] Image m_icon;
    [SerializeField] Text m_tooltipText;
    [SerializeField] Text m_costText;
    [SerializeField] RectTransform m_rectTransform;
    private bool m_isDrag;
    private CardDataBase m_database;
    private Vector2 m_defPos;

    public void Setup(CardDataBase dataBase)
    {
        SetBaseData(dataBase);
    }

    private void SetBaseData(CardDataBase database)
    {
        m_database = database;
        m_nameText.text = database.Name;
        m_icon.sprite = database.Icon;
        m_tooltipText.text = database.Tooltip;
        m_costText.text = database.Cost;
    }

    private void CardExecute(IDrop target)
    {
        target.GetDrop();
    }

    //以下インターフェース

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!m_isDrag)
            m_defPos = m_rectTransform.anchoredPosition;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_isDrag = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_rectTransform.anchoredPosition = new Vector2(eventData.position.x, eventData.position.y - 100);//カーソルがカード中央に来るように調整
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_rectTransform.anchoredPosition = m_defPos;
        m_isDrag = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    public void OnDrop(PointerEventData eventData)
    {
        var result = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, result);
        foreach (var hit in result)
        {
            IDrop target = hit.gameObject.GetComponent<IDrop>();
            if (target == null)
                return;
            CardExecute(target);
        }
    }
}
