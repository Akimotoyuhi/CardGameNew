using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EffectView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image m_image;
    [SerializeField] Text m_text;
    [SerializeField] List<SpriteSetting> m_spriteSettings;
    [System.Serializable]
    public class SpriteSetting
    {
        [SerializeField] EffectID m_effectID;
        [SerializeField] Sprite m_sprite;
        [SerializeField] Color m_color = Color.white;
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

    public void Setup(EffectBase effect)
    {
        m_text.text = effect.Turn.ToString();
        m_tooltip = effect.Tooltip;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GUIManager.InfoText = m_tooltip;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GUIManager.InfoText = "";
    }
}
