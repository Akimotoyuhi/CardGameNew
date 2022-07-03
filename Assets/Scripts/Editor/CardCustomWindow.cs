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

    public static void ShowWindow(CardDataBase cardData)
    {
        m_database = cardData;
        Setup(cardData);
        GetWindow(typeof(CardCustomWindow));
    }

    private void OnGUI()
    {
        m_name = GUILayout.TextField(m_name);
    }

    private static void Setup(CardDataBase database)
    {
        m_name = database.Name;
    }
}
#endif