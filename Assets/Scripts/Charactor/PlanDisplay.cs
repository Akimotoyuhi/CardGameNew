using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

/// <summary>
/// �G�s���\���\������N���X
/// </summary>
public class PlanDisplay : MonoBehaviour
{
    [SerializeField] Image m_planImage;
    [SerializeField] Text m_valueText;
    [SerializeField] List<PlanInfomation> m_planInfomantion;

    public void Setup(PlanData plandata)
    {
        m_planInfomantion.ForEach(info =>
        {
            if (plandata.PlanType == info.PlanType)
            {
                m_planImage.sprite = info.Sprite;
                m_planImage.color = info.Color;
                m_valueText.text = plandata.Text;
            }
        });
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