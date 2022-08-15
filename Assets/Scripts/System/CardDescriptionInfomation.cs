using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// �G�t�F�N�g��J�[�h���ʂ̐�����\������
/// </summary>
public class CardDescriptionInfomation : MonoBehaviour
{
    [System.Serializable]
    public class DescriptionText
    {
        [SerializeField, Tooltip("�p���̐���"), TextArea]
        string m_disposeText;
        [SerializeField, Tooltip("�󔖂̐���"), TextArea]
        string m_etherealText;
        [SerializeField, Tooltip("�X�g�b�N�̐���"), TextArea]
        string m_stockText;
        [SerializeField, Tooltip("����̐���"), TextArea]
        string m_releaseText;
        /// <summary>
        /// �J�[�h��������ݒ�
        /// </summary>
        /// <param name="cardDescriptionItem"></param>
        /// <returns></returns>
        public string GetDescriptionText(CardDescriptionItem cardDescriptionItem)
        {
            string ret = "";
            if (cardDescriptionItem.IsDispose)
                ret += m_disposeText + "\n";
            if (cardDescriptionItem.IsEthereal)
                ret += m_etherealText + "\n";
            if (cardDescriptionItem.IsStock)
                ret += m_stockText + "\n";
            if (cardDescriptionItem.IsRelease)
                ret += m_releaseText + "\n";
            return ret;
        }
    }
    [SerializeField] DescriptionText m_descriptionText;
    [SerializeField] Image m_panel;
    [SerializeField] Text m_displayText;
    private bool m_isActive;
    private string m_tooltip;
    public bool SetActive
    {
        set
        {
            if (value)
            {
                if (m_isActive)
                {
                    m_panel.gameObject.SetActive(true);
                }
            }
            else
            {
                m_panel.gameObject.SetActive(false);
            }
        }
    }

    public void Setup(CardDescriptionItem cardDescriptionItem, List<Command> commands)
    {
        m_tooltip = "";
        m_tooltip = m_descriptionText.GetDescriptionText(cardDescriptionItem);
        commands.ForEach(c =>
        {
            if (c.Effect != null)
            {
                c.Effect.ForEach(cc =>
                {
                    m_tooltip += cc.Tooltip + "\n";
                });
            }
        });
        if (m_tooltip.Length > 0)
            m_isActive = true;
        m_displayText.text = m_tooltip;
    }
}
