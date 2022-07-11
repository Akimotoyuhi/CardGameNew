using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;

public class CardCustomWindow : EditorWindow
{
    private static CardDataBase m_database;
    private static List<CardData.RaritySprite> m_raritySprite;
    private static List<CardData.TypeSprite> m_types;
    private static string m_name;
    private static string m_cost;
    private static string m_tooltip;
    private static string m_iconTexturePath = default;
    private static Sprite m_icon;
    private static UseType m_useType;
    private static Rarity m_rarity;
    private static List<CardType> m_type;
    private static Texture2D m_cardBackground;
    private static Texture2D m_background;
    private float m_settingAriaWidth = 200;
    private float m_settingAriaHeight = 20;
    private static float m_cardAriaWidth = 180;
    private static float m_cardAriaHeight = 260;
    private static float m_cardViewAriaSizeWidth = 300;
    private static float m_cardViewAriaSizeHeight = 250;
    private Vector2 m_scrollPos;
    private bool m_commandSettingToggleFlag;

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
        m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos, false, true,
            GUILayout.Width(m_settingAriaWidth + 30), GUILayout.Height(m_cardViewAriaSizeHeight + 50));
        {
            GUILayout.Label("カード名");
            m_name = GUILayout.TextField(m_name,
                GUILayout.Width(m_settingAriaWidth), GUILayout.Height(m_settingAriaHeight));

            GUILayout.Label("消費コスト\n(文字列の場合はプレイヤーの最大コスト)");
            m_cost = GUILayout.TextField(m_cost,
                GUILayout.Width(20), GUILayout.Height(20));

            GUILayout.Label("アイコン画像");
            if (GUILayout.Button("SetIconSprite"))
            {
                m_iconTexturePath = EditorUtility.OpenFilePanelWithFilters("画像を選択", Application.dataPath, new string[] { "Image files", "jpg,png" });
            }

            GUILayout.Label("使用対象");
            m_useType = (UseType)EditorGUILayout.EnumPopup(m_useType,
                GUILayout.Width(m_settingAriaWidth), GUILayout.Height(m_settingAriaHeight));

            GUILayout.Label("レア度");
            EditorGUI.BeginChangeCheck();
            m_rarity = (Rarity)EditorGUILayout.EnumPopup(m_rarity,
                GUILayout.Width(m_settingAriaWidth), GUILayout.Height(m_settingAriaHeight));
            if (EditorGUI.EndChangeCheck())
            {
                Graphics.ConvertTexture(GetTexture(m_rarity), m_cardBackground);
            }

            GUILayout.Label("ツールチップ");
            m_tooltip = GUILayout.TextArea(m_tooltip, GUILayout.Width(m_settingAriaWidth));

            GUILayout.Label("効果設定");
            m_commandSettingToggleFlag = EditorGUILayout.Foldout(m_commandSettingToggleFlag, "Commands");
            if (m_commandSettingToggleFlag)
            {
                HorizontalIndentAria(1, () => GUILayout.Label("Level1"));
            }
        }
        EditorGUILayout.EndScrollView();

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

                //アイコン画像の表示
                if (m_iconTexturePath.Length > 0)
                {
                    GUILayout.BeginArea(new Rect(m_cardAriaWidth / 4, 30, 90, 90), AssetDatabase.LoadAssetAtPath<Texture2D>(m_iconTexturePath.Substring(m_iconTexturePath.IndexOf("Assets"))));
                    GUILayout.EndArea();
                }

                //ツールチップの表示 https://zaki0929.github.io/page44.html
                GUILayout.BeginArea(new Rect(0, 120, m_cardAriaWidth, 120));
                style = new GUIStyle();
                GUIStyleState styleState = new GUIStyleState();
                styleState.textColor = Color.black;
                style.normal = styleState;
                style.wordWrap = true;
                GUILayout.Label(m_tooltip, style);
                GUILayout.EndArea();
            }
            GUILayout.EndArea();
        }
        GUILayout.EndArea();
    }

    private static void Setup(CardDataBase database, List<CardData.RaritySprite> raritySprite, List<CardData.TypeSprite> typeSprite)
    {
        m_name = database.Name;
        m_icon = database.Icon;
        m_iconTexturePath = AssetDatabase.GetAssetPath(m_icon);
        m_cost = database.Cost;
        m_useType = database.CardUseType;
        m_rarity = database.Rarity;
        m_type = database.CardType;
        m_tooltip = database.Tooltip;
        m_raritySprite = raritySprite;
        m_cardBackground = new Texture2D((int)m_cardAriaWidth, (int)m_cardAriaHeight);
        Graphics.ConvertTexture(GetTexture(m_rarity), m_cardBackground);
        var t = new Texture2D((int)m_cardViewAriaSizeWidth, (int)m_cardViewAriaSizeHeight);
        t.SetPixel(t.width, t.height, Color.white);
        m_background = t;
        m_types = typeSprite;
    }

    private void HorizontalIndentAria(int indentLevel, Action action)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(indentLevel * 10);
        action();
        GUILayout.EndHorizontal();
    }

    private static Texture2D GetTexture(Rarity rarity)
    {
        foreach (var r in m_raritySprite)
        {
            if (r.Rarity == rarity)
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

public enum CommandType
{
    Attack,
    Block,
    Effect
}