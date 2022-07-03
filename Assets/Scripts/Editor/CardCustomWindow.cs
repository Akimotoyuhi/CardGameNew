using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;

public class CardCustomWindow : EditorWindow
{
    private static CardDataBase m_database;
    private static string m_name;
    private static Sprite m_icon;

    public static void ShowWindow(CardDataBase cardData)
    {
        m_database = cardData;
        Setup(cardData);
        GetWindow(typeof(CardCustomWindow));
    }

    private void OnGUI()
    {
        m_name = GUILayout.TextField(m_name);
        //EditorGUILayout.LabelField(new GUIContent(Texture2D. m_icon), GUILayout.Height(200), GUILayout.Width(200));
    }

    private static void Setup(CardDataBase database)
    {
        m_name = database.Name;
        m_icon = database.Icon;
    }
}
#endif