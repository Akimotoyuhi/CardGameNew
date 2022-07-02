using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;

public class CardCustomWindow : EditorWindow
{
    private static CardDataBase m_database;

    private void OnEnable()
    {
        
    }

    [MenuItem("Custom/CardEdit")]
    public static void ShowWindow(CardDataBase cardData)
    {
        m_database = cardData;
        GetWindow(typeof(CardCustomWindow));
    }

    private void OnGUI()
    {
        GUILayout.Label(m_database.Name);
    }
}
#endif