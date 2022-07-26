using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    [SerializeField] RectTransform m_recrTransform;
    [SerializeField] Text m_text;
    [SerializeField] List<SelectTextColor> m_selectTextColor;
    private Sequence m_sequence;

    public void Setup(int value, Vector2 addPosition, float duration, DamageTextType textType, Ease ease, System.Action onCompleate = null)
    {
        m_text.text = value.ToString();
        SetColor(textType);
        m_sequence = DOTween.Sequence();
        m_sequence.Append(m_recrTransform.DOAnchorPos(
            new Vector2(m_recrTransform.anchoredPosition.x + addPosition.x, m_recrTransform.anchoredPosition.y + addPosition.y)
            , duration))
            .Join(m_text.DOColor(Color.clear, duration))
            .SetEase(ease)
            .OnComplete(() =>
            {
                if (onCompleate != null)
                    onCompleate();
                Destroy(gameObject);
            });
    }

    private void SetColor(DamageTextType damageTextType)
    {
        m_selectTextColor.ForEach(t =>
        {
            if (t.DamageTextType == damageTextType)
                m_text.color = t.Color;
        });
    }

    /// <summary>
    /// �A�j���[�V�����̒��f
    /// </summary>
    public void Kill()
    {
        m_sequence.Kill();
    }

    [System.Serializable]
    public class SelectTextColor
    {
        [SerializeField] DamageTextType m_textType;
        [SerializeField] Color m_color;
        public DamageTextType DamageTextType => m_textType;
        public Color Color => m_color;
    }
}

public enum DamageTextType
{
    Damage,
    Block,
    Heal,
}