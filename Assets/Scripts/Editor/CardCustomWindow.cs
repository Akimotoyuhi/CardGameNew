using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;

public class CardCustomWindow : EditorWindow
{
    private static CardDataBase m_database;
    private static List<CardData.RaritySprite> m_raritySprite;
    private static List<CardData.TypeSprite> m_types;
    private static string m_name;
    private static Sprite m_icon;
    private static Rarity m_rarity;
    private static List<CardType> m_type;
    private static Texture2D m_background;
    private static float m_settingAriaWidth = 200;
    private static float m_settingAriaHeight = 20;
    private static float m_cardAriaWidth = 180;
    private static float m_cardAriaHeight = 260;

    public static void ShowWindow(CardDataBase cardData, List<CardData.RaritySprite> raritySprite, List<CardData.TypeSprite> typeSprite)
    {
        m_database = cardData;
        Setup(cardData, raritySprite, typeSprite);
        GetWindow(typeof(CardCustomWindow));
    }

    private void OnGUI()
    {
        if (m_database == null)
            return;
        m_name = GUILayout.TextField(m_name,
            GUILayout.Width(m_settingAriaWidth), GUILayout.Height(m_settingAriaHeight));
        GUILayout.BeginArea(new Rect(m_settingAriaWidth + 50, 0, 500, 500));
        {
            GUILayout.Box(m_background,
                GUILayout.Width(m_cardAriaWidth), GUILayout.Height(m_cardAriaHeight));
        }
        GUILayout.EndArea();
    }

    private static void Setup(CardDataBase database, List<CardData.RaritySprite> raritySprite, List<CardData.TypeSprite> typeSprite)
    {
        m_name = database.Name;
        m_icon = database.Icon;
        m_rarity = database.Rarity;
        m_type = database.CardType;
        m_raritySprite = raritySprite;
        m_background = new Texture2D((int)m_cardAriaWidth, (int)m_cardAriaHeight);
        Graphics.ConvertTexture(GetTexture(m_rarity), m_background);
        m_types = typeSprite;
    }

    private static Texture2D GetTexture(Rarity rarity)
    {
        foreach (var r in m_raritySprite)
        {
            if (r.Rarity == m_database.Rarity)
            {
                return r.Sprite.texture;
            }    
        }
        return null;
    }

    private static List<Texture2D> GetTexture(CardType cardType)
    {
        List<Texture2D> ret = new List<Texture2D>();
        foreach (var myType in m_database.CardType)
        {
            foreach (var t in m_types)
            {
                if (myType == t.CardType)
                {
                    ret.Add(t.Sprite.texture);
                    continue;
                }
            }
        }
        return ret;
    }
}
#endif