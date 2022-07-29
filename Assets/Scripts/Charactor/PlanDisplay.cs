using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 敵行動予定を表示するクラス
/// </summary>
public class PlanDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image m_planImage;
    [SerializeField] Text m_valueText;
    [SerializeField] List<PlanInfomation> m_planInfomantion;
    private string m_tooltip;

    public void Setup(PlanData plandata)
    {
        m_planInfomantion.ForEach(info =>
        {
            if (plandata.PlanType == info.PlanType)
            {
                m_planImage.sprite = info.Sprite;
                m_planImage.color = info.Color;
                m_valueText.text = plandata.Text;
                m_tooltip = info.Tooltip;
                return;
            }
        });
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.Instance.SetInfoText = m_tooltip;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.Instance.SetInfoText = "";
    }

    [System.Serializable]
    public class PlanInfomation
    {
        [SerializeField] PlanType m_planType;
        [SerializeField] Sprite m_sprite;
        [SerializeField] Color m_color = Color.white;
        [SerializeField, TextArea] string m_tooltip;
        public PlanType PlanType => m_planType;
        public Sprite Sprite => m_sprite;
        public Color Color => m_color;
        public string Tooltip => m_tooltip;
    }
}

public struct PlanData
{
    public PlanType PlanType { get; set; }
    public string Text { get; set; }
}