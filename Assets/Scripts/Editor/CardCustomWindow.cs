using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;

public class CardCustomWindow : EditorWindow
{
    private static CardDataBase m_database;
    private static List<CardData.RaritySprite> m_raritySprite;
    private static string m_name;
    private static string m_cost;
    private static string m_tooltip;
    private static string m_cardTypeCountText;
    private static Sprite m_icon;
    private static UseType m_useType;
    private static Rarity m_rarity;
    private static Type m_commands;
    private static List<CardType> m_cardTypes;
    private static CardType[] m_cardTypesArray;
    private static Texture2D m_cardBackground;
    private static Texture2D m_background;
    private static Texture2D m_iconTexture;
    //private static int m_cardTypeCount;
    private float m_settingAriaWidth = 200;
    private float m_settingAriaHeight = 20;
    private static float m_cardAriaWidth = 180;
    private static float m_cardAriaHeight = 280;
    private static float m_cardViewAriaSizeWidth = 300;
    private static float m_cardViewAriaSizeHeight = 280;
    private Vector2 m_scrollPos;
    //private bool m_commandSettingToggleFlag;
    //private bool m_cardTypeToggleFlag;

    public static void ShowWindow(CardDataBase cardData, List<CardData.RaritySprite> raritySprite)
    {
        m_database = cardData;
        Setup(cardData, raritySprite);
        GetWindow(typeof(CardCustomWindow));
    }

    private void OnGUI()
    {
        if (m_database == null)
            return;
        //�ݒ荀�ڂ̕\��
        m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos, false, true,
            GUILayout.Width(m_settingAriaWidth + 30), GUILayout.Height(m_cardViewAriaSizeHeight));
        {
            GUILayout.Label("�J�[�h��");
            m_name = GUILayout.TextField(m_name,
                GUILayout.Width(m_settingAriaWidth), GUILayout.Height(m_settingAriaHeight));

            GUILayout.Label("����R�X�g\n(������̏ꍇ�̓v���C���[�̍ő�R�X�g)");
            m_cost = GUILayout.TextField(m_cost,
                GUILayout.Width(20), GUILayout.Height(20));

            GUILayout.Label("�c�[���`�b�v");
            m_tooltip = GUILayout.TextArea(m_tooltip, GUILayout.Width(m_settingAriaWidth));

            GUILayout.Label("�A�C�R���摜");
            if (GUILayout.Button("�摜��I��", GUILayout.Width(m_settingAriaWidth), GUILayout.Height(m_settingAriaHeight)))
            {
                string path = EditorUtility.OpenFilePanelWithFilters("�摜��I��", Application.dataPath, new string[] { "Image files", "jpg,png" });
                m_iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path.Substring(path.IndexOf("Assets")));
                m_icon = AssetDatabase.LoadAssetAtPath<Sprite>(path.Substring(path.IndexOf("Assets")));
            }

            GUILayout.Label("�g�p�Ώ�");
            m_useType = (UseType)EditorGUILayout.EnumPopup(m_useType,
                GUILayout.Width(m_settingAriaWidth), GUILayout.Height(m_settingAriaHeight));

            GUILayout.Label("���A�x");
            EditorGUI.BeginChangeCheck();
            m_rarity = (Rarity)EditorGUILayout.EnumPopup(m_rarity,
                GUILayout.Width(m_settingAriaWidth), GUILayout.Height(m_settingAriaHeight));
            if (EditorGUI.EndChangeCheck())
            {
                Graphics.ConvertTexture(GetTexture(m_rarity), m_cardBackground);
            }
        }
        EditorGUILayout.EndScrollView();

        //�ݒ�𔽉f������{�^��
        if (GUILayout.Button("����"))
        {
            AppryButton();
        }

        //�ݒ蒆�̃J�[�h��\������̈�
        GUILayout.BeginArea(new Rect(m_settingAriaWidth + 20, 0, m_cardViewAriaSizeWidth, m_cardViewAriaSizeHeight));
        {
            //�J�[�h�S��
            GUILayout.BeginArea(new Rect(30, 30, m_cardAriaWidth, m_cardAriaHeight), m_cardBackground);
            {
                //�R�X�g�\��
                GUIStyle style = new GUIStyle();
                style.fontSize = 20;
                style.alignment = TextAnchor.MiddleLeft;
                GUILayout.Label(m_cost, style);

                //���O�\��
                style = new GUIStyle();
                style.fontSize = 15;
                style.stretchWidth = true;
                style.alignment = TextAnchor.MiddleCenter;
                Rect rect = new Rect(0, 0, m_cardAriaWidth, 30);
                GUI.Label(rect, m_name, style);

                //�A�C�R���摜�̕\��
                if (m_iconTexture != null)
                {
                    GUILayout.BeginArea(new Rect(m_cardAriaWidth / 4, 30, 90, 90), m_iconTexture);
                    GUILayout.EndArea();
                }

                //�c�[���`�b�v�̕\�� https://zaki0929.github.io/page44.html
                GUILayout.BeginArea(new Rect(0, 125, m_cardAriaWidth, 120));
                style = new GUIStyle();
                GUIStyleState styleState = new GUIStyleState();
                styleState.textColor = Color.black;
                style.normal = styleState;
                style.fontSize = 15;
                style.wordWrap = true;
                GUILayout.Label(m_tooltip, style);
                GUILayout.EndArea();
            }
            GUILayout.EndArea();
        }
        GUILayout.EndArea();
    }

    private static void Setup(CardDataBase database, List<CardData.RaritySprite> raritySprite)
    {
        m_name = database.Name;
        m_icon = database.Icon;
        if (m_icon != null)
        {
            string icon = AssetDatabase.GetAssetPath(m_icon);
            m_iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(icon.Substring(icon.IndexOf("Assets")));
        }
        m_cost = database.Cost;
        m_useType = database.CardUseType;
        m_rarity = database.Rarity;
        m_tooltip = database.Tooltip;
        m_raritySprite = raritySprite;
        //m_cardTypesArray = m_cardTypes.ToArray();
        m_cardBackground = new Texture2D((int)m_cardAriaWidth, (int)m_cardAriaHeight);
        Graphics.ConvertTexture(GetTexture(m_rarity), m_cardBackground);
        var t = new Texture2D((int)m_cardViewAriaSizeWidth, (int)m_cardViewAriaSizeHeight);
        t.SetPixel(t.width, t.height, Color.white);
        m_background = t;
    }

    /// <summary>
    /// GUI��IndentLevel�����ɂ��炷
    /// </summary>
    private void HorizontalIndentAria(int indentLevel, Action action)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(indentLevel * 10);
        action();
        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// ���A���e�B�ɑΉ������J�[�h�̔w�i�摜��Texture�ɂ��ēn��
    /// </summary>
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

    //private static List<Texture2D> GetTexture(CardType cardType)
    //{
    //    List<Texture2D> ret = new List<Texture2D>();
    //    foreach (var myType in m_database.CardType)
    //    {
    //        foreach (var t in m_types)
    //        {
    //            if (myType == t.CardType)
    //            {
    //                ret.Add(t.Sprite.texture);
    //                continue;
    //            }
    //        }
    //    }
    //    return ret;
    //}

    /// <summary>
    /// �ݒ芮��
    /// </summary>
    private void AppryButton()
    {
        m_database.SetData(m_name, m_icon, m_cost, m_tooltip, m_useType, m_rarity, m_cardTypes);
        Close();
    }
}
#endif

public enum CommandType
{
    Attack,
    Block,
    Effect
}