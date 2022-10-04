using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// ���������G�t�F�N�g����ʏ�ɕ\������
/// </summary>
public class EffectDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image m_image;
    [SerializeField] Text m_text;
    [SerializeField] List<SpriteSetting> m_spriteSettings;
    /// <summary>���������G�t�F�N�g���̕\������摜�ƐF��ݒ肷��N���X</summary>
    [System.Serializable]
    public class SpriteSetting
    {
        [SerializeField] EffectID m_effectID;
        [SerializeField] Sprite m_sprite;
        [SerializeField] Color m_color = Color.white;
        public EffectID EffectID => m_effectID;
        public Image SetImage
        {
            set
            {
                value.sprite = m_sprite;
                value.color = m_color;
            }
        }
    }
    private string m_tooltip;

    public static EffectDisplay Init(EffectDisplay prefab, EffectBase effectBase)
    {
        EffectDisplay ret = Instantiate(prefab);
        ret.Setup(effectBase);
        return ret;
    }

    public void Setup(EffectBase effect)
    {
        m_text.text = effect.Turn.ToString();
        m_tooltip = effect.Tooltip;
        Debug.Log(m_tooltip);
        m_spriteSettings.ForEach(s =>
        {
            if (s.EffectID == effect.GetEffectID)
                s.SetImage = m_image;
        });
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Enter");
        GameManager.Instance.SetInfoText = m_tooltip;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Exit");
        GameManager.Instance.SetInfoText = "";
    }
}
