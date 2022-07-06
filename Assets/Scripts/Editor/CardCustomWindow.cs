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
    private static string m_cost;
    private static Sprite m_icon;
    private static Rarity m_rarity;
    private static List<CardType> m_type;
    private static Texture2D m_cardBackground;
    private static Texture2D m_background;
    private static float m_settingAriaWidth = 200;
    private static float m_settingAriaHeight = 20;
    private static float m_cardAriaWidth = 180;
    private static float m_cardAriaHeight = 260;
    private static float m_cardViewAriaSizeWidth = 300;
    private static float m_cardViewAriaSizeHeight = 250;

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
        //設定項目の表示
        m_name = GUILayout.TextField(m_name,
            GUILayout.Width(m_settingAriaWidth), GUILayout.Height(m_settingAriaHeight));
        m_cost = GUILayout.TextField(m_cost,
            GUILayout.Width(20), GUILayout.Height(20));

        //設定中のカードを表示する領域
        GUILayout.BeginArea(new Rect(m_settingAriaWidth + 20, 0, m_cardViewAriaSizeWidth, m_cardViewAriaSizeHeight));
        {
            //カード全体
            GUILayout.BeginArea(new Rect(30, 30, m_cardAriaWidth, m_cardAriaHeight), m_cardBackground);
            {
                //コスト表示
                GUIStyle style = new GUIStyle();
                style.fontSize = 20;
                style.alignment = TextAnchor.MiddleLeft;
                GUILayout.Label(m_cost, style);

                //名前表示
                style = new GUIStyle();
                style.fontSize = 15;
                style.stretchWidth = true;
                style.alignment = TextAnchor.MiddleCenter;
                Rect rect = new Rect(0, 0, m_cardAriaWidth, 30);
                GUI.Label(rect, m_name, style);
                //GUILayout.Box(m_icon.texture);
            }
            GUILayout.EndArea();
        }
        GUILayout.EndArea();
    }

    private static void Setup(CardDataBase database, List<CardData.RaritySprite> raritySprite, List<CardData.TypeSprite> typeSprite)
    {
        m_name = database.Name;
        m_icon = database.Icon;
        m_cost = database.Cost;
        m_rarity = database.Rarity;
        m_type = database.CardType;
        m_raritySprite = raritySprite;
        m_cardBackground = new Texture2D((int)m_cardAriaWidth, (int)m_cardAriaHeight);
        Graphics.ConvertTexture(GetTexture(m_rarity), m_cardBackground);
        var t = new Texture2D((int)m_cardViewAriaSizeWidth, (int)m_cardViewAriaSizeHeight);
        t.SetPixel(t.width, t.height, Color.white);
        m_background = t;
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