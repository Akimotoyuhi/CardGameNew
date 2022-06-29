using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;

public class CardCustomWindow : EditorWindow
{
    private CardData m_cardData;

    private void OnEnable()
    {
        
    }

    [MenuItem("Custom/CardEdit")]
    public static void ShowWindow()
    {
        GetWindow(typeof(CardCustomWindow));
    }

    private void OnGUI()
    {
        GUILayout.Label(m_cardData.name);
    }
}
#endif